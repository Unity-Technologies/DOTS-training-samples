using System;
using System.Collections.Generic;

namespace Unity.Entities.Editor
{
    struct PooledDictionary<TKey, TValue> : IDisposable
    {
        static class DictionaryPool
        {
            static readonly ObjectPool<Dictionary<TKey, TValue>> s_Pool = new ObjectPool<Dictionary<TKey, TValue>>(Clear, Clear);
            static void Clear(Dictionary<TKey, TValue> dictionary) => dictionary.Clear();

            public static Dictionary<TKey, TValue> Get(LifetimePolicy lifetime = LifetimePolicy.Frame)
            {
                return s_Pool.Get(lifetime);
            }

            public static void Release(Dictionary<TKey, TValue> toRelease)
            {
                s_Pool.Release(toRelease);
            }
        }

        public readonly Dictionary<TKey, TValue> Dictionary;

        internal static PooledDictionary<TKey, TValue> Make()
        {
            return new PooledDictionary<TKey, TValue>(DictionaryPool.Get());
        }

        public static implicit operator Dictionary<TKey, TValue>(PooledDictionary<TKey, TValue> d)
        {
            return d.Dictionary;
        }

        PooledDictionary(Dictionary<TKey, TValue> dictionary)
        {
            Dictionary = dictionary;
        }

        public void Dispose()
        {
            DictionaryPool.Release(Dictionary);
        }
    }
}
