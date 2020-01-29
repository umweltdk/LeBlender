using Newtonsoft.Json;
using System;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Lecoati.LeBlender.Extension.Models
{
    [TableName("LeBlenderProperty")]
    [PrimaryKey("Id", autoIncrement = true)]
    public class LeBlenderPropertyModel
    {
        [Column("Id")]
        [JsonProperty("id")]
        [PrimaryKeyColumn(AutoIncrement = true)]
        public int Id { get; set; }

        [Index(IndexTypes.UniqueNonClustered)]
        [Column("Guid")]
        [JsonProperty("guid")]
        public Guid Guid { get; set; }

        [Column("Name")]
        [JsonProperty("name")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string Name { get; set; }

        [Column("Alias")]
        [JsonProperty("alias")]
        public string Alias { get; set; }

        [Column("Description")]
        [JsonProperty("description")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string Description { get; set; }
        
        [Column("DataType")]
        [JsonProperty("dataType")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string DataType { get; set; }

        [Column("PropertyType")]
        [JsonProperty("propertyType")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string PropertyType { get; set; }

        [Column("SortOrder")]
        [JsonProperty("sortOrder")]
        public int SortOrder { get; set; }

        [JsonIgnore]
        [ForeignKey(typeof(LeBlenderConfigModel), Name = "LeBlenderConfigId")]
        [Index(IndexTypes.NonClustered, Name = "LeBlenderConfigId")]
        public int LeBlenderConfigId { get; set; }
    }
}