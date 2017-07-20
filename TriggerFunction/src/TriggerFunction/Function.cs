using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

using Amazon.Lambda.Core;
using Amazon.Lambda.S3Events;
using Amazon.S3;
using Nest;
using Newtonsoft.Json;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace TriggerFunction {

    //--- Types ---
    public class Function {

        //--- Constants ---
        private const string INDEX_NAME = "heroes";
        private const string TYPE_NAME = "hero";

        //--- Fields ---
        private readonly AmazonS3Client _s3Client;
        private readonly ElasticClient _esClient;

        //--- Constructors ---
        public Function() {
            _s3Client = new AmazonS3Client();
            var settings = new ConnectionSettings(new Uri(System.Environment.GetEnvironmentVariable("es_domain")));
            _esClient = new ElasticClient(settings);
        }

        //--- Methods ---
        public void FunctionHandler(S3Event e, ILambdaContext context) {
            var record = e.Records[0];
            Log($"received S3 event ({record.EventName})");
            if(record.EventName != "ObjectCreated:Put") {
                Log("skipping S3 event");
                return;
            }
            var bucket = record.S3.Bucket.Name;
            var key = record.S3.Object.Key;

            Log("fetching file from S3");
            string contents;
            using(var s3response = _s3Client.GetObjectAsync(bucket, key).Result) {
                Log("loading file to memory");
                var memoryStream = new MemoryStream();
                s3response.ResponseStream.CopyTo(memoryStream);
                contents = Encoding.UTF8.GetString(memoryStream.ToArray());
            }
            var lines = contents.Split('\n').ToArray();

            Log($"found {lines.Length:N0} rows");
            var heroes = lines.Select(line => line.Split('\t'))
                .Where(columns => columns.Length >= 15)
                .Select(columns => {
                    try {
                        return new Hero {
                            Id = int.Parse(columns[0]),
                            Name = columns[1],
                            Identity = columns[3],
                            Alignment = columns[4],
                            EyeColor = columns[5],
                            HairColor = columns[6],
                            Sex = columns[7],
                            Gsm = columns[9],
                            Appearances = columns[10],
                            FirstAppearance = columns[11],
                            Location = new Hero.GeoLocation {
                                Longitude = double.Parse(columns[13]),
                                Latitude = double.Parse(columns[14])
                            }
                        };
                    } catch(Exception ex) {
                        Log($"*** ERROR: {ex}\n    ROW: {string.Join(", ", columns)}");
                        return null;
                    }
                })
                .Where(hero => hero != null)
                .ToArray();

            // insert heroes in batches
            foreach(var batch in heroes
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / 500)
                .Select(x => x.Select(v => v.Value).ToList())
                .ToList()
            ) {
                _esClient.IndexMany(batch, "heroes", "hero");
            }
            Log($"inserted {heroes.Length:N0} records; skipped {lines.Length - heroes.Length:N0} rows");
        }

        private void Log(string text) {
            LambdaLogger.Log(text + "\n");
        }
    }
}
