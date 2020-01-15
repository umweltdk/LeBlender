using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;
using Umbraco.Core.PropertyEditors;
using Umbraco.Web;

namespace Lecoati.LeBlender.Extension.Models
{
    [TableName("LeBlenderConfig")]
    [PrimaryKey("Id", autoIncrement = true)]
    public class LeBlenderConfigModel
    {
        [Column("Id")]
        [JsonProperty("id")]
        [PrimaryKeyColumn(AutoIncrement = true)]
        public int Id { get; set; }

        [Column("Alias")]
        [JsonProperty("alias")]
        public string Alias { get; set; }

        [Column("HasProperties")]
        [JsonProperty("hasProperties")]
        public bool HasProperties { get; set; }

        [Column("Data")]
        [JsonProperty(PropertyName = "Data")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string Data { get; set; }

        [ResultColumn]
        [JsonProperty("dataTypeGuid")]
        public string DataTypeGuid { get; set; }

        [ResultColumn]
        [JsonProperty("value")]
        public object Value { get; set; }

        [JsonIgnore]
        [ForeignKey(typeof(LeBlenderGridEditorModel), Name = "LeBlenderGridEditorId")]
        [Index(IndexTypes.NonClustered, Name = "LeBlenderGridEditorId")]
        public int LeBlenderGridEditorId { get; set; }


        public T GetValue<T>()
        {

            //var targetContentType = Helper.GetTargetContentType();
            var targetDataType = Helper.GetTargetDataTypeDefinition(Guid.Parse(DataTypeGuid));

            var properyType = new PublishedPropertyType(Helper.GetTargetContentType(),
                new PropertyType(new DataTypeDefinition(targetDataType.PropertyEditorAlias)
                {
                    Id = targetDataType.Id
                }));

            // Try Umbraco's PropertyValueConverters
            var converters = PropertyValueConvertersResolver.Current.Converters.ToArray();
            foreach (var converter in converters.Where(x => x.IsConverter(properyType)))
            {
                // Convert the type using a found value converter
                var value2 = converter.ConvertDataToSource(properyType, Value, false);

                // If the value is of type T, just return it
                if (value2 is T)
                    return (T)value2;

                // If ConvertDataToSource failed try ConvertSourceToObject.
                var value3 = converter.ConvertSourceToObject(properyType, value2, false);

                // If the value is of type T, just return it
                if (value3 is T)
                    return (T)value3;

                // Value is not final value type, so try a regular type conversion aswell
                var convertAttempt = value2.TryConvertTo<T>();
                if (convertAttempt.Success)
                    return convertAttempt.Result;
            }

            // if already the requested type, return
            if (Value is T) return (T)Value;

            // if can convert to requested type, return
            var convert = Value.TryConvertTo<T>();
            if (convert.Success) return convert.Result;

            return default(T);

        }
    }
}