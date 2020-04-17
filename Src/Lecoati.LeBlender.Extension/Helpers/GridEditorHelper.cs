using Lecoati.LeBlender.Extension.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Lecoati.LeBlender.Extension.Helpers
{
    public class GridEditorHelper
    {
        public static IOrderedEnumerable<LeBlenderGridEditorModel> GetEditors()
        {
            var editors = RuntimeCacheHelper.GetCachedItem<IOrderedEnumerable<LeBlenderGridEditorModel>>("LeBlenderEditors");
            if (editors == null)
            {
                var dbHelper = new DatabaseHelper();
                editors = dbHelper.GetEditors();
                RuntimeCacheHelper.SetCacheItem("LeBlenderEditors", editors, 1);
            }
            return editors;
        }
    }
}