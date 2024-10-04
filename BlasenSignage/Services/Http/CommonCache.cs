using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlasenSignage.Services.Http
{
    public class CommonCache
    {
        public static CommonCache GetInstance()
        {
            if (instance == null)
            {
                instance = new CommonCache();
            }
            return instance;
        }


        public string GetAllCache()
        {
            var result = new StringBuilder();
            result.Append("[\n");
            foreach (var item in cache)
            {
                result.Append("  {\n");
                result.AppendFormat($"  \"key\": \"{item.Key}\",\n");
                result.AppendFormat($"  \"value\": \"{item.Value}\",\n");
                result.Append("  },\n");
            }
            result.Append("]\n");

            return result.ToString();
        }


        public bool GetCacheValue(string key, out string value)
        {
            return cache.TryGetValue(key, out value);
        }


        public void PutCacheValue(string key, string value)
        {
            cache[key] = value;
        }


        public bool DeleteCacheValue(string key, out string value)
        {
            return cache.TryRemove(key, out value);
        }


        private readonly ConcurrentDictionary<string, string> cache = new();
        private static CommonCache instance;
    }
}
