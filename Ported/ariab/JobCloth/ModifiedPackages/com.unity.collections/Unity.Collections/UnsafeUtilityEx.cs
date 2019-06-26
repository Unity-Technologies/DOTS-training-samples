using System;

namespace Unity.Collections.LowLevel.Unsafe
{
    unsafe public static class UnsafeUtilityEx
    {
        public static ref T AsRef<T>(void* ptr) where T : struct
        {
#if NET_DOTS
            return ref UnsafeUtility.AsRef<T>(ptr);
#else
            return ref System.Runtime.CompilerServices.Unsafe.AsRef<T>(ptr);
#endif
        }

        public static ref T ArrayElementAsRef<T>(void* ptr, int index) where T : struct
        {
#if NET_DOTS
            return ref UnsafeUtility.AsRef<T>((byte*)ptr + index * UnsafeUtility.SizeOf<T>());
#else
            return ref System.Runtime.CompilerServices.Unsafe.AsRef<T>((byte*)ptr + index * UnsafeUtility.SizeOf<T>());
#endif
        }

        public static void* RestrictNoAlias(void* ptr)
        {
            return ptr;
        }

        public static void MemSet(void* destination, byte value, int count)
        {
#if  UNITY_2019_3_OR_NEWER
            UnsafeUtility.MemSet(destination, value, count);
#else
            if (value == 0)
                UnsafeUtility.MemClear(destination, count);
            else
                for (int i = 0; i < count; ++i)
                    ((byte*) destination)[i] = value;
#endif
        }

        public static bool IsUnmanaged<T>()
        {
            return UnsafeUtility.IsUnmanaged<T>();
        }
    }
}
