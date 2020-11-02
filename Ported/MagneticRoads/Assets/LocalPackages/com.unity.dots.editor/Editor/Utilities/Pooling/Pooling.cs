using System;
using System.Collections.Generic;

namespace Unity.Entities.Editor
{
    static class Pooling
    {
        public static PooledList<T> GetList<T>()
        {
            return PooledList<T>.Make();
        }

        public static PooledList<T> ToPooledList<T>(this IEnumerable<T> source)
        {
            var pooled = GetList<T>();
            var list = pooled.List;
            list.AddRange(source);
            return pooled;
        }

        public static PooledQueue<T> GetQueue<T>()
        {
            return PooledQueue<T>.Make();
        }

        public static PooledQueue<T> ToPooledQueue<T>(this IEnumerable<T> source)
        {
            var pooled = GetQueue<T>();
            var queue = pooled.Queue;
            foreach (var element in source)
            {
                queue.Enqueue(element);
            }
            return pooled;
        }

        public static PooledHashSet<T> GetHashSet<T>()
        {
            return PooledHashSet<T>.Make();
        }

        public static PooledHashSet<T> ToPooledHashSet<T>(this IEnumerable<T> source)
        {
            var pooled = GetHashSet<T>();
            var set = pooled.Set;
            foreach (var element in source)
            {
                set.Add(element);
            }
            return pooled;
        }

        public static PooledDictionary<TKey, TValue> GetDictionary<TKey, TValue>()
        {
            return PooledDictionary<TKey, TValue>.Make();
        }

        public static PooledDictionary<TKey, TElement> ToPooledDictionary<TSource, TKey, TElement>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            Func<TSource, TElement> elementSelector)
        {
            var pooled = GetDictionary<TKey, TElement>();
            var dictionary = pooled.Dictionary;
            foreach (var item in source)
            {
                dictionary.Add(keySelector(item), elementSelector(item));
            }
            return pooled;
        }
    }
}
