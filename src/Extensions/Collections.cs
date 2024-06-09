using System;
using System.Collections.Generic;

namespace TSG_Library.Extensions
{
    public static partial class __Extensions_
    {
        #region Collections

        //[Obsolete(nameof(NotImplementedException))]
        //public static IDictionary<double, List<Face>> __ToILookIDict<T, K>(
        //    this IEnumerable<T> objects, Func<T, K> value)
        //{
        //    var dict = new Dictionary<double, List<Face>>();

        //    foreach(var obj in objects)
        //    {

        //    }


        //    throw new NotImplementedException();
        //}

        public static IDictionary<TKey, List<TValue>> __ToILookIDict<TKey, TValue>(
            this IEnumerable<TValue> source,
            Func<TValue, TKey> keySelector,
            IEqualityComparer<TKey> keyComparer = null)
        {
            // Creates the dictionary with the default equality comparer if one was not provided.
            var dictionary =
                new Dictionary<TKey, List<TValue>>(keyComparer ?? EqualityComparer<TKey>.Default);

            foreach (var value in source)
            {
                // Gets the key from the specified key selector.
                var key = keySelector(value);

                // Checks to see if the dictionary contains the {key}.
                if(!dictionary.ContainsKey(key))
                    // If it doesn't we need to add it with an initialized {List<TValue>}.
                    dictionary[key] = new List<TValue>();

                // Adds the current {value} to the specified {key} list.
                dictionary[key].Add(value);
            }

            return dictionary;
        }

        #endregion
    }
}