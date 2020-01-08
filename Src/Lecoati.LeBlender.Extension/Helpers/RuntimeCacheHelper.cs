using System;
using Umbraco.Core;

namespace Lecoati.LeBlender.Extension.Helpers
{
    public class RuntimeCacheHelper
    {
        public static T GetCachedItem<T>(string cacheKey)
        {
            var cachedItem = ApplicationContext.Current.ApplicationCache.RuntimeCache.GetCacheItem(cacheKey);
            return (T)cachedItem;
        }
        public static void SetCacheItem<T>(string cacheKey, T item, int days)
        {
            DeleteCacheItem(cacheKey);
            ApplicationContext.Current.ApplicationCache.RuntimeCache.InsertCacheItem(cacheKey, () => item, TimeSpan.FromDays(days));
        }
        public static void DeleteCacheItem(string cacheKey)
        {
            ApplicationContext.Current.ApplicationCache.RuntimeCache.ClearCacheItem(cacheKey);
        }
    }
}