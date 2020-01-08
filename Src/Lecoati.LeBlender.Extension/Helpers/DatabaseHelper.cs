using Lecoati.LeBlender.Extension.Models;
using Lecoati.LeBlender.Extension.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core;

namespace Lecoati.LeBlender.Extension.Helpers
{
    public class DatabaseHelper
    {

        internal static IList<GridEditor> GetEditors()
        {

            var tempList = new List<GridEditor>();

            var db = ApplicationContext.Current.DatabaseContext.Database;

            var gridEditors = db.Fetch<LeblenderGridEditorModel>("SELECT * FROM leBlenderGridEditors");


            if(gridEditors != null && gridEditors.Any())
            {
                foreach(var gridEditor in gridEditors)
                {
                    var config = new Dictionary<string, object>();

                    var editorProperties = db.Fetch<LeblenderPropertyModel>("WHERE GridEditorId = @0", gridEditor.Id);
                    if(editorProperties != null && editorProperties.Any())
                    {
                        editorProperties.ForEach(x => config.Add(x.Alias, x.Value));
                    }

                    var editorToAdd = new GridEditor
                    {
                        Alias = gridEditor.Alias,
                        Name = gridEditor.Name,
                        Icon = gridEditor.Icon,
                        Render = gridEditor.Render,
                        View = gridEditor.View,
                        Config = config
                    };

                    tempList.Add(editorToAdd);
                }
            }
            return tempList;
        }
    }
}