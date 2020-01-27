using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Lecoati.LeBlender.Extension.Models
{
    [TableName("LeBlenderGridEditor")]
    [PrimaryKey("Id", autoIncrement = true)]
    internal class LeBlenderGridEditorModel
    {

        public LeBlenderGridEditorModel()
        {
            Config = new Dictionary<string, object>();
        }

        [Column("Id")]
        [PrimaryKeyColumn(AutoIncrement = true)]
        [JsonProperty("id")]
        public int Id { get; set; }

        [Index(IndexTypes.UniqueNonClustered)]
        [Column("Guid")]
        [JsonProperty("guid")]
        public Guid Guid { get; set; }


        [Column("Name")]
        [JsonProperty("name", Required = Required.Always)]
        public string Name { get; set; }

        [Column("Alias")]
        [JsonProperty("alias", Required = Required.Always)]
        public string Alias { get; set; }

        [Column("View")]
        [JsonProperty("view", Required = Required.Always)]
        public string View { get; set; }

        [Column("Render")]
        [NullSetting(NullSetting = NullSettings.Null)]
        [JsonProperty("render")]
        public string Render { get; set; }

        [Column("Icon")]
        [JsonProperty("icon", Required = Required.Always)]
        public string Icon { get; set; }

        [Column("SortOrder")]
        [JsonProperty("sortOrder")]
        public int SortOrder { get; set; }

        [ResultColumn]
        [JsonProperty("config")]
        public IDictionary<string, object> Config { get; set; }

        protected bool Equals(LeBlenderGridEditorModel other)
        {
            return string.Equals(Alias, other.Alias);
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// true if the specified object  is equal to the current object; otherwise, false.
        /// </returns>
        /// <param name="obj">The object to compare with the current object. </param>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((LeBlenderGridEditorModel)obj);
        }

        /// <summary>
        /// Serves as a hash function for a particular type. 
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        public override int GetHashCode()
        {
            return Alias.GetHashCode();
        }
    }

}