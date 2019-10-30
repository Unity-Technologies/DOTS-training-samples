namespace Unity.Serialization
{
    unsafe struct UnsafeBuffer<T> where T : unmanaged
    {
        public T* Buffer;
        public int Length;
    }
}