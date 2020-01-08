//clear cache on publish
using System;
using System.Web.Mvc;
using System.Web.Routing;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Core.Publishing;
using System.Web;
using Umbraco.Core.Persistence;
using Lecoati.LeBlender.Extension.Helpers;
using Lecoati.LeBlender.Extension.Models.Database;

namespace Lecoati.LeBlender.Extension.Events
{
    public class UmbracoEvents : ApplicationEventHandler
    {

        protected override void ApplicationInitialized(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            base.ApplicationInitialized(umbracoApplication, applicationContext);

            RouteTable.Routes.MapRoute(
                "leblender",
                "umbraco/backoffice/leblender/helper/{action}",
                new
                {
                    controller = "Helper",
                }
            );
        }

        protected override void ApplicationStarting(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {

            var ctx = ApplicationContext.Current.DatabaseContext;
            var schemaHelper = new DatabaseSchemaHelper(ctx.Database, ApplicationContext.Current.ProfilingLogger.Logger, ctx.SqlSyntax);
            
            if (!schemaHelper.TableExist("leBlenderGridEditors"))
            {
                // Create LeBlender Database Table
                schemaHelper.CreateTable<LeblenderGridEditorModel>(false);
                if (!schemaHelper.TableExist("leBlenderProperties"))
                {
                    schemaHelper.CreateTable<LeblenderPropertyModel>(false);
                }
            }
            // Check if table exists
            else if (!schemaHelper.TableExist("leBlenderProperties"))
            {
                schemaHelper.CreateTable<LeblenderPropertyModel>(false);
            }
            DatabaseMigrationHelper.UpdateWithExistingEditors();


            // Upgrate default view path for LeBlender 1.0.0
            var gridConfig = HttpContext.Current.Server.MapPath("~/Config/grid.editors.config.js");
            if (System.IO.File.Exists(gridConfig))
            {
                try
                {
                    string readText = System.IO.File.ReadAllText(gridConfig);
                    if (readText.IndexOf("/App_Plugins/Lecoati.LeBlender/core/LeBlendereditor.html") > 0
                        || readText.IndexOf("/App_Plugins/Lecoati.LeBlender/editors/leblendereditor/LeBlendereditor.html") > 0
                        || readText.IndexOf("/App_Plugins/Lecoati.LeBlender/core/views/Base.cshtml") > 0
                        || readText.IndexOf("/App_Plugins/Lecoati.LeBlender/editors/leblendereditor/views/Base.cshtml") > 0
                        )
                    {
                        readText = readText.Replace("/App_Plugins/Lecoati.LeBlender/core/LeBlendereditor.html", "/App_Plugins/LeBlender/editors/leblendereditor/LeBlendereditor.html")
                            .Replace("/App_Plugins/Lecoati.LeBlender/editors/leblendereditor/LeBlendereditor.html", "/App_Plugins/LeBlender/editors/leblendereditor/LeBlendereditor.html")
                            .Replace("/App_Plugins/Lecoati.LeBlender/core/views/Base.cshtml", "/App_Plugins/LeBlender/editors/leblendereditor/views/Base.cshtml")
                            .Replace("/App_Plugins/Lecoati.LeBlender/editors/leblendereditor/views/Base.cshtml", "/App_Plugins/LeBlender/editors/leblendereditor/views/Base.cshtml");
                        System.IO.File.WriteAllText(gridConfig, readText);
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Error<Helper>("Enable to upgrate LeBlender 1.0.0", ex);
                }
            }

            base.ApplicationStarting(umbracoApplication, applicationContext);
            PublishingStrategy.Published += PublishingStrategy_Published;
        }

        private void PublishingStrategy_Published(IPublishingStrategy sender, Umbraco.Core.Events.PublishEventArgs<Umbraco.Core.Models.IContent> e)
        {
            ApplicationContext.Current.ApplicationCache.RuntimeCache.ClearCacheByKeySearch("LEBLENDEREDITOR");
        }

    }
}