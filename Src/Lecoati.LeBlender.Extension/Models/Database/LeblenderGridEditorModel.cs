using System.Collections.Generic;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Lecoati.LeBlender.Extension.Models.Database
{
    [TableName("leBlenderGridEditors")]
    [PrimaryKey("Id", autoIncrement = true)]
    internal class LeblenderGridEditorModel
    {
        [Column("Id")]
        [PrimaryKeyColumn(AutoIncrement = true)]
        public int Id { get; set; }

        [Column("Name")]
        
        public string Name { get; set; }

        [Column("Alias")]
        public string Alias { get; set; }

        [Column("View")]
        public string View { get; set; }

        [Column("Render")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string Render { get; set; }

        [Column("Icon")]
        public string Icon { get; set; }
        
        [ResultColumn]
        public ICollection<LeblenderPropertyModel> Properties { get; set; }
    }

    [TableName("leBlenderProperties")]
    [PrimaryKey("Id", autoIncrement = true)]
    internal class LeblenderPropertyModel
    {
        [Column("Id")]
        [PrimaryKeyColumn(AutoIncrement = true)]
        public int Id { get; set; }

        [Column("Alias")]
        public string Alias { get; set; }

        [Column("Value")]
        [Length(4000)]
        public string Value { get; set; }

        [ForeignKey(typeof(LeblenderGridEditorModel), Name = "LeBlenderGridEditorId")]
        [Index(IndexTypes.NonClustered, Name = "LeBlenderGridEditorId")]
        public int GridEditorId { get; set; }
    }
}