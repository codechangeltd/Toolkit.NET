namespace System.Collections.Specialized
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    
    public static class NameValueCollectionExtensions
    {
        /// <summary>
        /// Converts the name value collection into an IEnumerable collection of key value pairs
        /// </summary>
        /// <param name="col">The name value collection</param>
        /// <returns>An enumeration of key value pairs in the name value collection</returns>
        public static IEnumerable<KeyValuePair<string, string>> AsEnumerable(this NameValueCollection col)
        {
            foreach (var key in col.AllKeys)
            {
                yield return new KeyValuePair<string, string>(key, col[key]);
            }
        }

        /// <summary>
        /// Converts the name value collection into a dictionary
        /// </summary>
        /// <param name="col">The name value collection</param>
        /// <returns>A dictionary representing the name value collection</returns>
        public static Dictionary<string, string> ToDictionary(this NameValueCollection col)
        {
            var dict = new Dictionary<string, string>();

            foreach (var k in col.AllKeys)
            {
                dict.Add(k, col[k]);
            }

            return dict;
        }

        /// <summary>
        /// Converts the name value collection into a dictionary
        /// </summary>
        /// <typeparam name="TKey">The key type</typeparam>
        /// <typeparam name="TValue">The value type</typeparam>
        /// <param name="col">The name value collection</param>
        /// <returns>A dictionary representing the name value collection</returns>
        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this NameValueCollection col)
        {
            var dict = new Dictionary<TKey, TValue>();
            var keyConverter = TypeDescriptor.GetConverter(typeof(TKey));
            var valueConverter = TypeDescriptor.GetConverter(typeof(TValue));

            foreach (string name in col)
            {
                var key = (TKey)keyConverter.ConvertFromString(name);
                var value = (TValue)valueConverter.ConvertFromString(col[name]);

                dict.Add(key, value);
            }

            return dict;
        }

        /// <summary>
        /// Inserts an item into the name value collection at the specified index position
        /// </summary>
        /// <param name="col">The name value collection to update</param>
        /// <param name="index">The index to insert the new item into</param>
        /// <param name="name">The items name</param>
        /// <param name="value">The items value</param>
        public static void Insert(this NameValueCollection col, int index, string name, string value)
        {
            if (index < 0 || index > col.Count)
            {
                throw new ArgumentOutOfRangeException();
            }

            if (col.GetKey(index) == value)
            {
                col.Add(name, value);
            }
            else
            {
                var items = new List<KeyValuePair<string, string>>();
                int size = col.Count;

                for (int i = index; i < size; i++)
                {
                    var key = col.GetKey(index);

                    items.Add(new KeyValuePair<string, string>(key, col.Get(index)));

                    col.Remove(key);
                }

                col.Add(name, value);

                foreach (var item in items)
                {
                    col.Add(item.Key, item.Value);
                }
            }
        }

        /// <summary>
        /// Inserts a blank item into the name value collection specified
        /// </summary>
        /// <param name="col">The name value collection to update</param>
        public static void InsertBlank(this NameValueCollection col)
        {
            if (col.Count == 0)
            {
                col.Add(String.Empty, String.Empty);
            }
            else if (false == col.AllKeys.Contains(String.Empty))
            {
                col.Insert(0, String.Empty, String.Empty);
            }
        }
    }
}
