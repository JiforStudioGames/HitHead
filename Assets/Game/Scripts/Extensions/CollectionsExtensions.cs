using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UniRx;
using Random = UnityEngine.Random;

namespace Extensions
{
    public static class CollectionsExtensions
    {
        public static T GetRandom<T>(this T[] array)
        {
            return array[Random.Range(0, array.Length)];
        }
        
        public static T GetRandom<T>(this IReadOnlyList<T> list)
        {
            if(list == null || list.Count == 0)
            {
                return default;
            }
            
            return list[Random.Range(0, list.Count)];
        }
        
        public static KeyValuePair<TKey, TValue> GetRandom<TKey, TValue>(this IReadOnlyReactiveDictionary<TKey, TValue> dictionary)
        {
            if (dictionary == null || dictionary.Count == 0)
            {
                return default;
            }
            
            int index = Random.Range(0, dictionary.Count);
            return dictionary.ElementAt(index);
        }
        
        public static T GetRandom<T>(this Array array)
        {
            return ((T[])array).GetRandom();
        }
        
        public static bool InBounds<T>(this int index, ICollection<T> collection)
        {
            if (collection == null)
                return false;

            return index >= 0 && index < collection.Count;
        }
        
        public static bool InBounds<T>(this int index, T[] array)
        {
            if (array == null)
                return false;

            return index >= 0 && index < array.Length;
        }
        
        public static bool InBounds<T>(this int index, IEnumerable<T> enumerable)
        {
            if (enumerable == null)
                return false;

            return index >= 0 && index < enumerable.Count();
        }
        
        public static TSource MaxBy<TSource, TKey>(this IEnumerable<TSource> collection, Func<TSource, TKey> keySelector) where TKey : IComparable<TKey> =>
            collection.Select(item => new ValueTuple<TSource, TKey>(item, keySelector(item)))
                .Aggregate((max, next) => next.Item2.CompareTo(max.Item2) > 0 ? next : max).Item1;

        public static TSource MinBy<TSource, TKey>(this IEnumerable<TSource> collection, Func<TSource, TKey> keySelector) where TKey : IComparable<TKey> {
            return collection.Select(item => new ValueTuple<TSource, TKey>(item, keySelector(item)))
                .Aggregate((min, next) => next.Item2.CompareTo(min.Item2) < 0 ? next : min).Item1;
        }

        public static void Map<TTo, TFrom>(
            this ICollection<TTo> to,
            IEnumerable<TFrom> from,
            Func<TTo, TFrom, bool> comparator,
            Func<TTo, TFrom, TTo> mapper)
            where TTo : class
        {
            var buffer = new List<TTo>(to);
            foreach (TTo a in buffer)
            {
                bool shouldRemove = true;
                foreach (TFrom b in from)
                {
                    if (comparator(a, b))
                    {
                        TTo result = mapper(a, b);
                        shouldRemove = result == null;
                        break;
                    }
                }
                if (shouldRemove)
                {
                    to.Remove(a);
                }
            }

            foreach (TFrom b in from)
            {
                bool shouldAdd = true;
                foreach (TTo a in to)
                {
                    if (comparator(a, b))
                    {
                        shouldAdd = false;
                        break;
                    }
                }
                if (shouldAdd)
                {
                    TTo result = mapper(null, b);
                    if (result != null)
                    {
                        to.Add(result);
                    }
                }
            }
        }
    }
}