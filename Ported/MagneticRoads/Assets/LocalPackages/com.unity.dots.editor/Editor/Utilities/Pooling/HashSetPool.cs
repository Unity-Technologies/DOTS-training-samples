using System;
using System.Collections.Generic;

namespace Unity.Entities.Editor
{
    struct PooledHashSet<T> : IDisposable
    {
        static class HashSetPool
        {
            static readonly ObjectPool<HashSet<T>> s_Pool = new ObjectPool<HashSet<T>>(Clear, Clear);
            static void Clear(HashSet<T> set) => set.Clear();

            public static HashSet<T> Get(LifetimePolicy lifetime = LifetimePolicy.Frame)
            {
                return s_Pool.Get(lifetime);
            }

            public static void Release(HashSet<T> toRelease)
            {
                s_Pool.Release(toRelease);
            }
        }

        public readonly HashSet<T> Set;

        internal static PooledHashSet<T> Make()
        {
            return new PooledHashSet<T>(HashSetPool.Get());
        }

        public static implicit operator HashSet<T>(PooledHashSet<T> pooled)
        {
            return pooled.Set;
        }

        PooledHashSet(HashSet<T> set)
        {
            Set = set;
        }

        public void Dispose()
        {
            HashSetPool.Release(Set);
        }
    }
}
