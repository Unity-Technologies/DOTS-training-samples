using System;
using System.Collections.Generic;

namespace Unity.Entities.Editor
{
    struct PooledQueue<T> : IDisposable
    {
        static class QueuePool
        {
            static readonly ObjectPool<Queue<T>> s_Pool = new ObjectPool<Queue<T>>(Clear, Clear);
            static void Clear(Queue<T> queue) => queue.Clear();

            public static Queue<T> Get(LifetimePolicy lifetime = LifetimePolicy.Frame)
            {
                return s_Pool.Get(lifetime);
            }

            public static void Release(Queue<T> toRelease)
            {
                s_Pool.Release(toRelease);
            }
        }

        public readonly Queue<T> Queue;

        internal static PooledQueue<T> Make()
        {
            return new PooledQueue<T>(QueuePool.Get());
        }

        public static implicit operator Queue<T>(PooledQueue<T> pooled)
        {
            return pooled.Queue;
        }

        PooledQueue(Queue<T> queue)
        {
            Queue = queue;
        }

        public void Dispose()
        {
            if (null != Queue)
            {
                QueuePool.Release(Queue);
            }
        }
    }
}
