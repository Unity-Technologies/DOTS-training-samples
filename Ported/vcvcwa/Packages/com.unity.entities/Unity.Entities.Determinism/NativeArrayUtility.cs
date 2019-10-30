using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace Unity.Entities.Determinism
{
    internal static class NativeArrayUtility
    {
        public static int LengthInBytes<T>(NativeArray<T> data) where T : struct
        {
            var sizeOfT = UnsafeUtility.SizeOf<T>();
            return sizeOfT * data.Length;
        }

        public static bool IsInvalidOrEmpty<T>(NativeArray<T> data) where T : struct
        {
            return (!data.IsCreated) || (0 == data.Length);
        }

        public static NativeArray<T> CreateCopy<T>(NativeArray<T> source, Allocator targetAllocator) where T : struct
        {
            var copy = new NativeArray<T>(source.Length, targetAllocator, NativeArrayOptions.UninitializedMemory);
            source.CopyTo(copy); 
            return copy;
        }
        
        public static NativeArray<byte> CreateOwningCopy(NativeView view, Allocator allocator)
        {
            var copy = new NativeArray<byte>(view.LengthInBytes, allocator, NativeArrayOptions.UninitializedMemory);
            unsafe
            {
                UnsafeUtility.MemCpy(copy.GetUnsafePtr(), view.Ptr, view.LengthInBytes);
            }
            return copy;
        }
    }

}
