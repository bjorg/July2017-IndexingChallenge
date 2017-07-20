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
        private readonly Uri _esDomain;

        //--- Constructors ---
        public Function() {
            _s3Client = new AmazonS3Client();
            _esDomain = new Uri(System.Environment.GetEnvironmentVariable("es_domain"));
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
            var lines = contents.Split('\n');

            Log($"found {lines.Length:N0} rows");
            var skipped = 0;
            foreach(var line in lines) {
                var columns = line.Split('\t');
                if(columns.Length < 15) {
                    ++skipped;
                }
                try {
                    Insert(new Hero {
                        Id = TryParseInt(columns[0]),
                        Name = columns[1],
                        Identity = columns[3],
                        Alignment = columns[4],
                        EyeColor = columns[5],
                        HairColor = columns[6],
                        Sex = columns[7],
                        Gsm = columns[9],
                        Appearances = columns[10],
                        FirstAppearance = columns[11],
                        Year = TryParseInt(columns[12]),
                        Location = new Hero.GeoLocation {
                            Longitude = TryParseDouble(columns[13]),
                            Latitude = TryParseDouble(columns[14])
                        }
                    });
                } catch(Exception ex) {
                    Log($"*** ERROR: {ex}");
                    ++skipped;
                }
            }
            Log($"inserted {lines.Length - skipped:N0} records; skipped {skipped:N0} rows");

            int TryParseInt(string text) {
                if(!int.TryParse(text, out int value)) {
                    Log($"*** ERROR: not an int value: '{text}'");
                }
                return value;
            }

            double TryParseDouble(string text) {
                if(!double.TryParse(text, out double value)) {
                    Log($"*** ERROR: not a double value: '{text}'");
                }
                return value;
            }
        }

        private void Insert(Hero hero) {
            var settings = new ConnectionSettings(_esDomain);
            var client = new ElasticClient(settings);
            var status = client.Index(hero, i => i.Index("heroes").Type("hero"));
        }

        private void Log(string text) {
            LambdaLogger.Log(text + "\n");
        }
    }
}
