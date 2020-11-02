namespace Unity.Entities.Editor
{
    static class Pool<T> where T : class, IPoolable, new()
    {
        static readonly ObjectPool<T> s_Pool = new ObjectPool<T>(NoOp, OnRelease);

        static void NoOp(T item) { }

        static void OnRelease(T item) => item.Reset();

        public static T GetPooled(LifetimePolicy lifetime = LifetimePolicy.Permanent)
        {
            return s_Pool.Get(lifetime);
        }

        public static void Release(T t)
        {
            t.Reset();
            s_Pool.Release(t);
        }
    }
}
