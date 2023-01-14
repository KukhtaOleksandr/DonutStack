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
        public static Dictionary<TKey, TValue> RemoveNonUnique<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, Dictionary<TKey, TValue> secondDictionary)
        {
            List<TKey> keys = new();
            foreach (var d in dictionary)
            {
                foreach (var sd in secondDictionary)
                {
                    if (d.Key.Equals(sd.Key))
                    {
                        keys.Add(d.Key);
                        break;
                    }
                }
            }
            foreach(var k in keys)
                dictionary.Remove(k);
            return dictionary;
        }
    }
}