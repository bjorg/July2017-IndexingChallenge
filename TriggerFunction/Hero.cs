/*
 * MIT License
 *
 * Copyright (c) 2017 Yuri Gorokhov, Oscar Cornejo
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

using Newtonsoft.Json;

namespace TriggerFunction {
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

}