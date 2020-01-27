using Newtonsoft.Json;

namespace Lecoati.LeBlender.Extension.Models.Manifest
{
    class Editor
    {

        [JsonProperty("view", Required = Required.Always)]
        public string View { get; set; }

    }
}
