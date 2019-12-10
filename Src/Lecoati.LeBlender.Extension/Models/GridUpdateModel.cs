using Newtonsoft.Json;

namespace Lecoati.LeBlender.Extension.Models
{
    public class GridUpdateModel
    {
        [JsonProperty(PropertyName = "oldValue")]
        public string OldValue { get; set; }
        [JsonProperty(PropertyName = "newValue")]
        public string NewValue { get; set; }
    }
}