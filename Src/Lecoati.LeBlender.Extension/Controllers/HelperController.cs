using Lecoati.LeBlender.Extension.Helpers;
using Lecoati.LeBlender.Extension.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using System.Xml.Linq;
using Umbraco.Core.Logging;
using Umbraco.Web.Mvc;

namespace Lecoati.LeBlender.Extension.Controllers
{
    public class HelperController : UmbracoAuthorizedController
    {
        [ValidateInput(false)]
        [HttpPost]
        public ActionResult GetPartialViewResultAsHtmlForEditor()
        {
            var modelStr = Request["model"];
            var view = Request["view"];
            dynamic model = JsonConvert.DeserializeObject(modelStr);
            return View("/views/Partials/" + view + ".cshtml", model);
        }

        [HttpPost]
        public ActionResult DeleteEditor(int id)
        {
            var message = $"Successfully deleted editor from the database";
            try
            {
                var dbHelper = new DatabaseHelper();
                dbHelper.DelteGridEditor(id);
                var editors = dbHelper.GetEditors();
                RuntimeCacheHelper.SetCacheItem("LeBlenderEditors", editors, 1);
            }
            catch (Exception ex)
            {
                message = $"Error while trying to delete GridEditor with id: {id} from the database";
                LogHelper.Error<HelperController>($"Error while trying to delete editor with id: {id} from the database", ex);
            }
            return Content(message);
        }


        [HttpPost]
        [ValidateInput(false)]
        public ActionResult DeleteAllEditors(string editors)
        {
            try
            {
                var gridEditors = JsonConvert.DeserializeObject<List<LeBlenderGridEditorModel>>(editors);
                if(gridEditors != null && gridEditors.Any())
                {
                    var dbHelper = new DatabaseHelper();
                    foreach(var gridEditor in gridEditors)
                    {
                        dbHelper.DelteGridEditor(gridEditor.Id);
                    }
                    var updatedEditors = dbHelper.GetEditors();
                    RuntimeCacheHelper.SetCacheItem("LeBlenderEditors", updatedEditors, 1);
                }
                return Content("Deleted all editors from database");
            }
            catch (Exception ex)
            {
                LogHelper.Error<HelperController>($"Error while trying to perform action: Delete All Editors", ex);
                return Content($"Error while trying to delete all editors. Error: {ex.Message}");
            }
        }


        [HttpPost]
        public ActionResult UpdateGridSortOrder(Dictionary<int, int> items)
        {
            var message = "Updated grid sortorder";
            try
            {
                var dbHelper = new DatabaseHelper();
                if(items.Count > 0)
                {
                    foreach(var item in items)
                    {
                        // Key = id, Value = SortOrder
                        dbHelper.UpdateGridSortOrder(item.Key, item.Value);
                    }
                }

                // Update runtimecache
                var editors = dbHelper.GetEditors();
                RuntimeCacheHelper.SetCacheItem("LeBlenderEditors", editors, 1);
            }
            catch (Exception ex)
            {
                message = "Error while trying to update grid sortorder";
                LogHelper.Error<HelperController>("Error while trying to update grid sortorder in the database", ex);
            }
            return Json(new { Message = message });
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult UpdateEditor(string editor)
        {
            var message = "Grid editor has been saved";
            try
            {
                var gridEditor = JsonConvert.DeserializeObject<LeBlenderGridEditorModel>(editor);
                var dbHelper = new DatabaseHelper();
                var LeBlenderGridEditorId = dbHelper.InsertOrUpdateGridEditor(gridEditor);

                if (gridEditor.Config.Count > 0)
                {
                    dbHelper.InsertOrUpdateConfig(LeBlenderGridEditorId, gridEditor.Config);
                }

                // Update runtimecache
                var editors = dbHelper.GetEditors();
                RuntimeCacheHelper.SetCacheItem("LeBlenderEditors", editors, 1);
            }
            catch (Exception ex)
            {
                LogHelper.Error<HelperController>($"Error while trying to update editor", ex);
                message = $"{ex.Message} - {ex.InnerException?.Message}";
            }
            return Json(new { Message = message });
        }

        [ValidateInput(false)]
        public ActionResult GetEditors()
        {
            // Check if they exist in cache
            var editors = RuntimeCacheHelper.GetCachedItem<IOrderedEnumerable<LeBlenderGridEditorModel>>("LeBlenderEditors");
            if(editors == null)
            {
                var dbHelper = new DatabaseHelper();
                editors = dbHelper.GetEditors();
                RuntimeCacheHelper.SetCacheItem("LeBlenderEditors", editors, 1);
            }
            return Content(JsonConvert.SerializeObject(editors));
        }

        [HttpGet]
        [ValidateInput(false)]
        public ActionResult GetTransferUrls()
        {
            var urls = new List<string>();
            // Check if there is a courier.config containing any remote urls
            var configFile = HttpContext.Server.MapPath("~/config/courier.config");
            if (System.IO.File.Exists(configFile))
            {
                var doc = XDocument.Load(configFile);
                var repos = doc.Root.Element("repositories").Descendants("repository").Descendants("url").Select(x => x.Value).ToList();
                if (repos.Any())
                {
                    urls.AddRange(repos);
                }
            }
            // Check if there are any remote urls in web.config
            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings.Get("LeBlender:TransferUrls")))
            {
                var configUrls = ConfigurationManager.AppSettings.Get("LeBlender:TransferUrls").Split(',');
                if (configUrls.Any())
                {
                    urls.AddRange(configUrls);
                }
            }
            return Content(JsonConvert.SerializeObject(urls.Distinct()));
        }
    }
}