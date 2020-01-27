using Lecoati.LeBlender.Extension.Helpers;
using umbraco;
using umbraco.BusinessLogic.Actions;
using Umbraco.Core;
using Umbraco.Core.Services;
using Umbraco.Web.Models.Trees;
using Umbraco.Web.Mvc;
using Umbraco.Web.Trees;

namespace Lecoati.LeBlender.Extension.Controllers
{

    [PluginController("LeBlender")]
    [Umbraco.Web.Trees.Tree("developer", "GridEditorManager", "Grid Editors", iconClosed: "icon-doc")]
    public class LeBlenderEditorManagerTreeController : TreeController
    {
        protected override Umbraco.Web.Models.Trees.MenuItemCollection GetMenuForNode(string id, System.Net.Http.Formatting.FormDataCollection queryStrings)
        {
            var textService = ApplicationContext.Services.TextService;
            var deleteText = textService.Localize($"actions/{ActionDelete.Instance.Alias}");

            var leBlenderCourierItem = new MenuItem
            {
                Alias = "transferEditor",
                Name = "Transfer Editors",
                Icon = "umb-deploy"
            };

            var menu = new MenuItemCollection();
            if (id == Constants.System.Root.ToInvariantString())
            {
                var createText = textService.Localize($"actions/{ActionNew.Instance.Alias}");
                var sortText = textService.Localize($"actions/{ActionSort.Instance.Alias}");
                var refreshNodeText = textService.Localize($"actions/{ActionRefresh.Instance.Alias}");
                var deleteAll = new MenuItem
                {
                    Alias = "delete",
                    Name = "Delete All Editors",
                    Icon = "delete"
                };

                deleteAll.AdditionalData.Add("DeleteAll", true);

                // root actions              
                leBlenderCourierItem.AdditionalData.Add("TransferAll", true);
                menu.Items.Add(leBlenderCourierItem);
                menu.Items.Add<CreateChildEntity, ActionNew>(createText);
                menu.Items.Add<ActionSort>(sortText);
                menu.Items.Add<RefreshNode, ActionRefresh>(refreshNodeText, true);
                menu.Items.Add(deleteAll);
                return menu;
            }
            leBlenderCourierItem.Name = "Transfer Editor";
            menu.Items.Add(leBlenderCourierItem);
            menu.Items.Add<ActionDelete>(deleteText);
            return menu;
        }

        protected override Umbraco.Web.Models.Trees.TreeNodeCollection GetTreeNodes(string id, System.Net.Http.Formatting.FormDataCollection queryStrings)
        {
            var nodes = new TreeNodeCollection();
            if (id == "-1")
            {
                var databaseHelper = new DatabaseHelper();
                var editors = databaseHelper.GetEditors();

                foreach (var editor in editors)
                {
                    nodes.Add(this.CreateTreeNode(editor.Alias, id, queryStrings, editor.Name, editor.Icon, false));
                }

                return nodes;
            }

            return nodes;
        }
    }
}
