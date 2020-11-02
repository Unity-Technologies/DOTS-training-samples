namespace Unity.Entities.Editor
{
    interface IPoolable
    {
        void Reset();
        void ReturnToPool();
    }
}
