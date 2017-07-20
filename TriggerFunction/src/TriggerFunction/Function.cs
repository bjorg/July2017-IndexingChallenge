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
            var bucket = e.Records[0].S3.Bucket.Name;
            var key = e.Records[0].S3.Object.Key;

            var s3response = _s3Client.GetObjectAsync(bucket, key).Result;

            var memoryStream = new MemoryStream();
            s3response.ResponseStream.CopyTo(memoryStream);
            var contents = Encoding.UTF8.GetString(memoryStream.ToArray());

            var lines = contents.Split('\n');
            foreach(var line in lines) {
                var columns = line.Split('\t');
            }
        }

        private void Insert(Hero hero) {
            // just do it!
        }
    }
}
