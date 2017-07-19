using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

using Amazon.Lambda.Core;
using Newtonsoft.Json;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace TriggerFunction {

    //--- Types ---
    public class Hero {

        public class GeoLocation {
            
            [JsonProperty("lon")]
            public double Longitude { get; set; }
            
            [JsonProperty("lat")]
            public double Latitude { get; set; }
        }

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("urlslug")]
        public string UrlSlug { get; set; }

        [JsonProperty("identity")]
        public string Identity { get; set; }

        [JsonProperty("alignment")]
        public string Alignment { get; set; }

        [JsonProperty("eye_color")]
        public string EyeColor { get; set; }

        [JsonProperty("hair_color")]
        public string HairColor { get; set; }

        [JsonProperty("sex")]
        public string Sex { get; set; }

        [JsonProperty("gsm")]
        public string Gsm { get; set; }

        [JsonProperty("appearances")]
        public string Appearances { get; set; }

        [JsonProperty("first_appearance")]
        public string FirstAppearance { get; set; }

        [JsonProperty("year")]
        public int Year  { get; set; }

        [JsonProperty("location")]
        public GeoLocation Location { get; set; }

        public override string ToString() {
            return $"{Name} | {UrlSlug} | {Identity} | {Alignment} | {EyeColor} | {HairColor} | {Sex} | {Gsm} | {Appearances} | {FirstAppearance} | {Year} | {Location.Longitude} | {Location.Latitude}";
        }
    }

    public class Function {

        //--- Constants ---
        private const string INDEX_NAME = "heroes";
        private const string TYPE_NAME = "hero";

        //--- Class Fields ---
        private static readonly Uri _esDomain = new Uri("https://TODO");
        
        //--- Methods ---
        public void FunctionHandler(Hero hero, ILambdaContext context) {
            LambdaLogger.Log($"received hero: {hero.ToString()}");
            
            //TODO: index to elastic search
        }
    }
}
