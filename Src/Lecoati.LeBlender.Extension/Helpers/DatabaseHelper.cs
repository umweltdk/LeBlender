using Lecoati.LeBlender.Extension.Models;
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
    public class DatabaseHelper
    {
        private readonly DatabaseContext dbContext;

        public DatabaseHelper()
        {
            dbContext = ApplicationContext.Current.DatabaseContext;
        }

        internal void CreateOrUpdateTables()
        {
            try
            {
                var schema = new DatabaseSchemaHelper(dbContext.Database, ApplicationContext.Current.ProfilingLogger.Logger, dbContext.SqlSyntax);
                if (!schema.TableExist("LeBlenderGridEditor"))
                {
                    schema.CreateTable<LeBlenderGridEditorModel>(false);
                }
                if (!schema.TableExist("LeBlenderConfig"))
                {
                    schema.CreateTable<LeBlenderConfigModel>(false);
                }
                if (!schema.TableExist("LeBlenderProperty"))
                {
                    schema.CreateTable<LeBlenderPropertyModel>(false);
                }

                // Read from the original LeBlender.editors.config.js file
                var editors = Helper.GetLeBlenderGridEditors(false);

                if (editors != null && editors.Any())
                {
                    for (int i = 0; i < editors.Count(); i++)
                    {
                        var current = editors[i];
                        current.SortOrder = i;
                        object LeBlenderGridEditorId = null;
                        if (dbContext.Database.FirstOrDefault<LeBlenderGridEditorModel>("WHERE Alias = @0", current.Alias) != null)
                        {
                            LeBlenderGridEditorId = dbContext.Database.FirstOrDefault<LeBlenderGridEditorModel>("WHERE Alias = @0", current.Alias).Id;
                        }
                        else
                        {
                            LeBlenderGridEditorId = dbContext.Database.Insert("LeBlenderGridEditor", "Id", true, current);
                        }
                        if (current.Config.Count > 0)
                        {
                            foreach (var config in current.Config)
                            {
                                var alias = config.Key;
                                if (dbContext.Database.FirstOrDefault<LeBlenderConfigModel>("WHERE Alias = @0 AND LeBlenderGridEditorId = @1", alias, LeBlenderGridEditorId) == null)
                                {
                                    var hasProperties = alias == "editors";

                                    var LeBlenderConfigId = dbContext.Database.Insert("LeBlenderConfig", "Id", true, new LeBlenderConfigModel
                                    {
                                        Alias = alias,
                                        Data = hasProperties ? null : config.Value.ToString(),
                                        HasProperties = hasProperties,
                                        LeBlenderGridEditorId = int.Parse(LeBlenderGridEditorId.ToString())
                                    });

                                    if (hasProperties)
                                    {
                                        var properties = JsonConvert.DeserializeObject<List<LeBlenderPropertyModel>>(config.Value.ToString());
                                        if (properties.Count > 0)
                                        {
                                            for (int j = 0; j < properties.Count; j++)
                                            {
                                                properties[j].LeBlenderConfigId = int.Parse(LeBlenderConfigId.ToString());
                                                properties[j].SortOrder = j;
                                                dbContext.Database.Insert("LeBlenderProperty", "Id", properties[j]);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    var gridConfig = HttpContext.Current.Server.MapPath("~/Config/grid.editors.config.js");
                    if (File.Exists(gridConfig))
                    {
                        LogHelper.Info<DatabaseHelper>($"Updated Database with existing GridEditors - Renaming old LeblenderGridEditor file({gridConfig})");
                        File.Move(gridConfig, gridConfig.TrimEnd(".js") + "_backup" + ".js");
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error<DatabaseHelper>("Error while trying to create and update LeBlender.GridEditor table in Umbraco database", ex);
            }
        }


        internal IOrderedEnumerable<LeBlenderGridEditorModel> GetEditors(bool onlyLeBlenderEditor = false)
        {
            var editors = new List<LeBlenderGridEditorModel>();
            var gridEditors = dbContext.Database.Fetch<LeBlenderGridEditorModel>("SELECT * FROM LeBlenderGridEditor");

            if (gridEditors != null && gridEditors.Any())
            {
                foreach (var gridEditor in gridEditors)
                {
                    var editorProperties = dbContext.Database.Fetch<LeBlenderConfigModel>("WHERE LeBlenderGridEditorId = @0", gridEditor.Id);
                    var isLeblenderEditor = false;
                    var dict = new Dictionary<string, object>();
                    foreach (var editorProperty in editorProperties)
                    {
                        if (editorProperty.HasProperties)
                        {
                            isLeblenderEditor = true;
                            var editorConfigValues = dbContext.Database.Fetch<LeBlenderPropertyModel>("WHERE LeBlenderConfigId = @0", editorProperty.Id).OrderBy(x => x.SortOrder);
                            dict.Add(editorProperty.Alias, editorConfigValues);
                        }
                        else
                        {
                            dict.Add(editorProperty.Alias, editorProperty.Data);
                        }
                    }
                    gridEditor.Config = dict;
                    if (onlyLeBlenderEditor)
                    {
                        if (isLeblenderEditor)
                        {
                            editors.Add(gridEditor);
                        }
                    }
                    else
                    {
                        editors.Add(gridEditor);
                    }
                }
            }
            return editors.OrderBy(x => x.SortOrder);
        }

        internal LeBlenderGridEditorModel GetGridEditor(string alias)
        {
            var editor = dbContext.Database.FirstOrDefault<LeBlenderGridEditorModel>("WHERE Alias = @0", alias);
            if(editor != null)
            {
                var editorProperties = dbContext.Database.Fetch<LeBlenderConfigModel>("WHERE LeBlenderGridEditorId = @0", editor.Id);
                var dict = new Dictionary<string, object>();
                foreach (var editorProperty in editorProperties)
                {
                    if (editorProperty.HasProperties)
                    {
                        var editorConfigValues = dbContext.Database.Fetch<LeBlenderPropertyModel>("WHERE LeBlenderConfigId = @0", editorProperty.Id).OrderBy(x => x.SortOrder);
                        dict.Add(editorProperty.Alias, editorConfigValues);
                    }
                    else
                    {
                        dict.Add(editorProperty.Alias, editorProperty.Data);
                    }
                }
                editor.Config = dict;
                return editor;
            }
            return null;
        }

        internal int InsertOrUpdateGridEditor(LeBlenderGridEditorModel editor)
        {
            try
            {
                if (editor.DeletedPropertyIds.Count > 0)
                {
                    editor.DeletedPropertyIds.ForEach(id => DeleteItem<LeBlenderPropertyModel>(id));
                }

                if (editor.OldAlias != null && editor.OldAlias != editor.Alias)
                {
                    Helper.UpdateGridNodes(editor.OldAlias, editor.Alias);
                }


                var existingEditor = dbContext.Database.FirstOrDefault<LeBlenderGridEditorModel>("WHERE Alias = @0", editor.Alias);
                if (existingEditor != null)
                {
                    existingEditor.Alias = editor.Alias;
                    existingEditor.Name = editor.Name;
                    existingEditor.Render = editor.Render;
                    existingEditor.View = editor.View;
                    existingEditor.Icon = editor.Icon;
                    existingEditor.SortOrder = editor.SortOrder;
                    dbContext.Database.Save(existingEditor);
                    return existingEditor.Id;
                }
                else
                {
                    var LeBlenderGridEditorId = dbContext.Database.Insert("LeBlenderGridEditor", "Id", true, editor);
                    return int.Parse(LeBlenderGridEditorId.ToString());
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error<DatabaseHelper>($"Error while trying to update grideditor with alias {editor.OldAlias}", ex);
            }
            return 0;
        }

        internal void InsertOrUpdateConfig(int leBlenderGridEditorId, IDictionary<string, object> configs)
        {
            foreach (var config in configs)
            {
                var existing = dbContext.Database.FirstOrDefault<LeBlenderConfigModel>("WHERE LeBlenderGridEditorId = @0 AND Alias = @1", leBlenderGridEditorId, config.Key);
                var hasProperties = config.Key == "editors";
                if (existing == null)
                {
                    var LeBlenderConfigId = dbContext.Database.Insert("LeBlenderConfig", "Id", new LeBlenderConfigModel
                    {
                        Alias = config.Key,
                        Data = hasProperties ? null : config.Value.ToString(),
                        LeBlenderGridEditorId = leBlenderGridEditorId,
                        HasProperties = hasProperties,
                    });
                    if (hasProperties)
                    {
                        InsertOrUpdateProperty(int.Parse(LeBlenderConfigId.ToString()), JsonConvert.DeserializeObject<List<LeBlenderPropertyModel>>(config.Value.ToString()), leBlenderGridEditorId);
                    }
                }
                else
                {
                    if (hasProperties)
                    {
                        existing.HasProperties = hasProperties;
                        InsertOrUpdateProperty(existing.Id, JsonConvert.DeserializeObject<List<LeBlenderPropertyModel>>(config.Value.ToString()), leBlenderGridEditorId);
                    }
                    else
                    {
                        existing.HasProperties = hasProperties;
                        existing.Data = config.Value.ToString();
                    }
                    dbContext.Database.Save(existing);
                }
            }
        }

        internal void InsertOrUpdateProperty(int leBlenderConfigId, List<LeBlenderPropertyModel> properties, int leBlenderGridEditorId)
        {
            var databaseProperties = dbContext.Database.Fetch<LeBlenderPropertyModel>("WHERE LeBlenderConfigId = @0", leBlenderConfigId);
            foreach (var property in properties)
            {
                var existing = databaseProperties.FirstOrDefault(x => x.Id == property.Id);
                if (existing != null)
                {
                    if(property.OldAlias != null && property.Alias != property.OldAlias)
                    {
                        var gridEditor = dbContext.Database.FirstOrDefault<LeBlenderGridEditorModel>("WHERE Id = @0", leBlenderGridEditorId);
                        if(gridEditor != null)
                        {
                            Helper.UpdateGridNodes(property.OldAlias, property.Alias, gridEditor.Alias);
                        }
                        else
                        {
                            LogHelper.Warn<DatabaseHelper>($"Could not update grid nodes, because GridEditor with Id: {leBlenderGridEditorId} does not exists in the database");
                        }
                    }

                    existing.Alias = property.Alias;
                    existing.DataType = property.DataType;
                    existing.Name = property.Name;
                    existing.PropertyType = property.PropertyType;
                    existing.Description = property.Description;
                    existing.SortOrder = property.SortOrder;
                    dbContext.Database.Save(existing);
                }
                else
                {
                    property.LeBlenderConfigId = leBlenderConfigId;
                    property.SortOrder = databaseProperties.Count;
                    dbContext.Database.Insert("LeBlenderProperty", "Id", property);
                }
            }
        }

        internal void DelteGridEditor(int id)
        {
            var gridEditorConfigs = dbContext.Database.Fetch<LeBlenderConfigModel>("WHERE LeBlenderGridEditorId = @0", id);
            // If GridEditor has any configs, we need to delete these first, because of a foreign key constraint
            if (gridEditorConfigs.Count > 0)
            {
                foreach (var gridConfig in gridEditorConfigs)
                {
                    // If the GridConfig has any properties, we need to delete these first, because of a foreign key constraint
                    if (gridConfig.HasProperties)
                    {
                        var gridConfigProperties = dbContext.Database.Fetch<LeBlenderPropertyModel>("WHERE LeBlenderConfigId = @0", gridConfig.Id);
                        gridConfigProperties.ForEach(property => DeleteItem<LeBlenderPropertyModel>(property.Id));
                    }
                    DeleteItem<LeBlenderConfigModel>(gridConfig.Id);
                }
            }
            DeleteItem<LeBlenderGridEditorModel>(id);
        }

        internal void UpdateGridSortOrder(int id, int index)
        {
            var itemToUpdate = dbContext.Database.FirstOrDefault<LeBlenderGridEditorModel>("WHERE Id = @0", id);
            if (itemToUpdate != null)
            {
                itemToUpdate.SortOrder = index;
                dbContext.Database.Save(itemToUpdate);
            }
        }

        internal void DeleteItem<T>(int id)
        {
            dbContext.Database.Delete<T>("WHERE Id = @0", id);
        }
    }
}