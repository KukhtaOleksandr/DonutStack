using System.Collections.Generic;
using Level;

namespace Extensions
{
    public static class DictionaryExtensions
    {
        public static Dictionary<TKey, TValue> AddUnique<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            foreach (var d in dictionary)
            {
                if (d.Key.Equals(key))
                {
                    dictionary[key] = value;
                    return dictionary;
                }
            }
            dictionary.Add(key, value);
            return dictionary;
        }
    }
}