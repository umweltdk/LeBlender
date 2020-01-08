using Newtonsoft.Json;
using System.Collections.Generic;

namespace Lecoati.LeBlender.Extension.Models.Manifest
{
    class Prevalue
    {

        [JsonProperty("fields")]
        public IEnumerable<Field> Fields { get; set; }

    }
}
