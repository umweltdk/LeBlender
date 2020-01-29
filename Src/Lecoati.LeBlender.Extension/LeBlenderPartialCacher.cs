﻿using System;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using Umbraco.Core;
using Umbraco.Core.Cache;

namespace Lecoati.LeBlender.Extension
{
    public static class LeBlenderPartialCacher
    {

        public static IHtmlString LeBlenderCachedPartial(
                        this HtmlHelper htmlHelper,
                        string partialViewName,
                        object model,
                        int cachedSeconds,
                        string guid,
                        ViewDataDictionary viewData = null
            )
        {
            var finalCacheKey = Helper.BuildCacheKey(guid);

            return ApplicationContext.Current.ApplicationCache.RuntimeCache.GetCacheItem<IHtmlString>(
                finalCacheKey,
                () => htmlHelper.Partial(partialViewName, model, viewData),
                new TimeSpan(0, 0, 0, cachedSeconds),
                false,
                CacheItemPriority.NotRemovable //not removable, the same as macros (apparently issue #27610)
            );
        }

    }
}