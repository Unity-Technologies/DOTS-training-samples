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

        [Obsolete("RestrictNoAlias has been deprecated by the Burst compiler, and its usage is a no-op.")]
        public static void* RestrictNoAlias(void* ptr)
        {
            return ptr;
        }
    }
}
