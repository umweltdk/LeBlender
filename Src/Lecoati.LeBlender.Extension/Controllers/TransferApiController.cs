using Lecoati.LeBlender.Extension.Helpers;
using Lecoati.LeBlender.Extension.Models;
using Newtonsoft.Json;
using System;
using System.Web.Http;
using System.Web.Mvc;
using Umbraco.Core.Logging;

namespace Lecoati.LeBlender.Extension.Controllers
{
    public class TransferController : Controller
    {
        [System.Web.Http.HttpPost]
        public ActionResult TransferEditor([FromBody]string editor)
        {
            var message = "Remote DB has been updated";
            try
            {
                var ed = Request["editor"];
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
                message = $"Error while receiving editor for database storage. Message: {ex.Message}";
                LogHelper.Error<HelperController>(message, ex);
            }
            Response.AppendHeader("Access-Control-Allow-Origin", "*");
            return Content(message);
        }
    }
}