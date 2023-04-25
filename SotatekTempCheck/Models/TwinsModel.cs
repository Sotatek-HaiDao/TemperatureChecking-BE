using Newtonsoft.Json;

namespace SotatekTempCheck.Models
{
    public class TwinsModel
    {
        [JsonProperty("$dtId")]
        public string Id { get; set; }
        public double Temperature { get; set; }
        [JsonProperty("$etag")]
        public string ETag { get; set; }
        [JsonProperty("$metadata")]
        public Metadata Metadata { get; set; }
    }

    public class Metadata
    {
        [JsonProperty("$model")]
        public string Model { get; set; }
        [JsonProperty("$lastUpdateTime")]
        public DateTime lastUpdateTime { get; set; }
    }

    public class Temperature
    {
        public DateTime lastUpdateTime { get; set; }
    }
}
