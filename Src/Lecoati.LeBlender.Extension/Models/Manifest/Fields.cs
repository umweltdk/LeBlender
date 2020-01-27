﻿using Newtonsoft.Json;

namespace Lecoati.LeBlender.Extension.Models.Manifest
{
    class Field
    {

        [JsonProperty("label", Required = Required.Always)]
        public string Label { get; set; }

        [JsonProperty("key", Required = Required.Always)]
        public string Key { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("view", Required = Required.Always)]
        public string View { get; set; }

        [JsonProperty("advanced")]
        public bool Advanced { get; set; }

    }
}
