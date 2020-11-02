using System;
using System.Collections.Generic;

namespace Unity.Entities.Editor
{
    struct PooledList<T> : IDisposable
    {
        static class ListPool
        {
            static readonly ObjectPool<List<T>> s_Pool = new ObjectPool<List<T>>(Clear, Clear);
            static void Clear(List<T> list) => list.Clear();

            public static List<T> Get(LifetimePolicy lifetime = LifetimePolicy.Frame)
            {
                return s_Pool.Get(lifetime);
            }

            public static void Release(List<T> toRelease)
            {
                s_Pool.Release(toRelease);
            }
        }

        public readonly List<T> List;

        internal static PooledList<T> Make()
        {
            return new PooledList<T>(ListPool.Get());
        }

        public static implicit operator List<T>(PooledList<T> pooled)
        {
            return pooled.List;
        }

        PooledList(List<T> list)
        {
            List = list;
        }

        public void Dispose()
        {
            if (null != List)
            {
                ListPool.Release(List);
            }
        }
    }
}
