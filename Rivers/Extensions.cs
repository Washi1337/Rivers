using System.Collections.Generic;
using System.Linq;

namespace Rivers
{
    internal static class Extensions
    {
        public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key,
            TValue defaultValue)
        {
            return dict.TryGetValue(key, out var value) ? value : defaultValue;
        }

    }
}