using Lecoati.LeBlender.Extension.Helpers;
using Lecoati.LeBlender.Extension.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using Umbraco.Core.Logging;

namespace Lecoati.LeBlender.Extension.Controllers
{
    public class TransferController : Controller
    {
        [ValidateInput(false)]
        [HttpPost]
        public ActionResult TransferEditor(string editor)
        {
            if (IsValidDomain(Request.Url))
            {
                Response.Headers.Remove("Access-Control-Allow-Origin");
                Response.AddHeader("Access-Control-Allow-Origin", Request.UrlReferrer.GetLeftPart(UriPartial.Authority));

                Response.Headers.Remove("Access-Control-Allow-Credentials");
                Response.AddHeader("Access-Control-Allow-Credentials", "true");

                Response.Headers.Remove("Access-Control-Allow-Methods");
                Response.AddHeader("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS");
            }
            else
            {
                Response.StatusCode = 403;
                return Content("Forbidden");
            }

            var message = "Grideditor has been transferred successfully";
            try
            {
                var gridEditor = JsonConvert.DeserializeObject<LeBlenderGridEditorModel>(editor);
                if (gridEditor != null)
                {
                    var dbHelper = new DatabaseHelper();
                    var gridEditorId = dbHelper.InsertOrUpdateGridEditor(gridEditor);
                    if (gridEditor.Config.Count > 0)
                    {
                        dbHelper.InsertOrUpdateConfig(gridEditorId, gridEditor.Config);
                    }

                    var editors = dbHelper.GetEditors();
                    RuntimeCacheHelper.SetCacheItem("LeBlenderEditors", editors, 1);
                }
                
            }
            catch (Exception ex)
            {
                message = $"Error while transferring editor for database storage. Message: {ex.Message}";
                LogHelper.Error<HelperController>(message, ex);
            }
            return Content(message);
        }

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult TransferAllEditors(string editors)
        {
            
            if (IsValidDomain(Request.Url))
            {
                Response.Headers.Remove("Access-Control-Allow-Origin");
                Response.AddHeader("Access-Control-Allow-Origin", Request.UrlReferrer.GetLeftPart(UriPartial.Authority));

                Response.Headers.Remove("Access-Control-Allow-Credentials");
                Response.AddHeader("Access-Control-Allow-Credentials", "true");

                Response.Headers.Remove("Access-Control-Allow-Methods");
                Response.AddHeader("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS");
            }
            else
            {
                Response.StatusCode = 403;
                return Content("Forbidden");
            }

            var message = "Grideditors has been transferred successfully";
            try
            {
                var gridEditors = JsonConvert.DeserializeObject<List<LeBlenderGridEditorModel>>(editors);
                if (gridEditors != null && gridEditors.Any())
                {
                    var dbHelper = new DatabaseHelper();
                    foreach(var gridEditor in gridEditors)
                    {
                        var gridEditorId = dbHelper.InsertOrUpdateGridEditor(gridEditor);
                        if (gridEditor.Config.Count > 0)
                        {
                            dbHelper.InsertOrUpdateConfig(gridEditorId, gridEditor.Config);
                        }
                    }

                    var savedEditors = dbHelper.GetEditors();
                    RuntimeCacheHelper.SetCacheItem("LeBlenderEditors", savedEditors, 1);
                }

            }
            catch (Exception ex)
            {
                message = $"Error while transferring editor for database storage. Message: {ex.Message}";
                LogHelper.Error<HelperController>(message, ex);
            }
            return Content(message);
        }

        private bool IsValidDomain(Uri url)
        {
            var allowedDomains = ConfigurationManager.AppSettings.Get("LeBlender:AllowedDomains").Split(',');
            return allowedDomains.Any(allowedUrl => url.Host.EndsWith(allowedUrl));
        }
    }
}