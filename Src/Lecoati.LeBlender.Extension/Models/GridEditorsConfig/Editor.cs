using Newtonsoft.Json;

namespace Lecoati.LeBlender.Extension.Models.GridEditorsConfig
{
    class Editor
    {

        [JsonProperty("name", Required = Required.Always)]
        public string Name { get; set; }

        [JsonProperty("alias", Required = Required.Always)]
        public string Alias { get; set; }

        [JsonProperty("view", Required = Required.Always)]
        public string View { get; set; }

        [JsonProperty("icon", Required = Required.Always)]
        public string Icon { get; set; }

    }
}
