using Newtonsoft.Json;
using System.Collections.Generic;

namespace Lecoati.LeBlender.Extension.Models
{
    [JsonObject]
    public class LeBlenderModel
    {
        [JsonProperty("value")]
        public IEnumerable<LeBlenderValue> Items { get; set; }
    }
}