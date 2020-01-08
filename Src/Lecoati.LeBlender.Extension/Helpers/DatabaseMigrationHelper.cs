using Lecoati.LeBlender.Extension.Models;
using Lecoati.LeBlender.Extension.Models.Database;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Core.Persistence;

namespace Lecoati.LeBlender.Extension.Helpers
{
    internal class DatabaseMigrationHelper
    {
        internal static void UpdateWithExistingEditors()
        {
            try
            {

                var existing = Helper.GetLeBlenderGridEditors(false);

                if (existing != null && existing.Any())
                {
                    var database = ApplicationContext.Current.DatabaseContext.Database;
                    foreach (var editor in existing)
                    {
                        object gridEditorId = null;
                        if (database.FirstOrDefault<LeblenderGridEditorModel>("WHERE Alias = @0", editor.Alias) != null)
                        {
                            gridEditorId = database.FirstOrDefault<LeblenderGridEditorModel>("WHERE Alias = @0", editor.Alias).Id;
                        }
                        else
                        {
                            gridEditorId = database.Insert("leBlenderGridEditors", "Id", true, new LeblenderGridEditorModel
                            {
                                Name = editor.Name,
                                Alias = editor.Alias,
                                Icon = editor.Icon,
                                Render = editor.Render,
                                View = editor.View
                            });
                        }
                        if (editor.Config.Count > 0)
                        {
                            foreach (var property in editor.Config)
                            {
                                var alias = property.Key;
                                if (database.FirstOrDefault<LeblenderPropertyModel>("WHERE Alias = @0 AND GridEditorId = @1", alias, gridEditorId) == null)
                                {
                                    database.Insert("leBlenderProperties", "Id", true, new LeblenderPropertyModel
                                    {
                                        Alias = alias,
                                        Value = property.Value.ToString(),
                                        GridEditorId = int.Parse(gridEditorId.ToString())
                                    });
                                }
                            }
                        }
                    }

                    var gridConfig = HttpContext.Current.Server.MapPath("~/Config/grid.editors.config.js");
                    LogHelper.Info<DatabaseMigrationHelper>($"Updated Database with existing GridEditors - Renaming old LeblenderGridEditor file({gridConfig})");
                    if (File.Exists(gridConfig))
                    {
                        File.Move(gridConfig, gridConfig.TrimEnd(".js") + "_beforeDB" + ".js");
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error<DatabaseMigrationHelper>("Error while trying to create and update LeBlender.GridEditor table in Umbraco database", ex);
            }
        }
    }

}