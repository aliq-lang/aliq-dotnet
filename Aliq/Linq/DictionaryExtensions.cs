using System;
using System.Collections.Generic;

namespace Aliq.Linq
{
    public static class DictionaryExtensions
    {
        public static T GetOrCreate<K, TB, T>(
            this IDictionary<K, TB> map, K key, Func<T> create)
            where T : TB
        {
            if (map.TryGetValue(key, out var resultB))
            {
                return (T)resultB;
            }
            else
            {
                var result = create();
                map[key] = result;
                return result;
            }
        }                        
    }
}
