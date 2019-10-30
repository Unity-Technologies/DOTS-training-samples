using System;
using System.Diagnostics;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;

namespace Unity.Entities.Determinism
{
    internal struct NativeView
    {
        [NativeDisableUnsafePtrRestriction, NoAlias]
        public readonly unsafe byte* Ptr;
        public readonly int LengthInBytes;

        public bool IsEmpty => 0 == LengthInBytes;
        
        public unsafe NativeView(byte* ptr, int lengthInBytes)
        {
            LengthInBytes = lengthInBytes;
            Ptr = ptr;
            CheckValidOrThrow();
        }

        [Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
        unsafe void CheckValidOrThrow()
        {
            if ((LengthInBytes > 0) && (null == Ptr))
            {
                throw new ArgumentException("Array has Length > 0 but Data is null");
            }

            if (LengthInBytes < 0)
            {
                throw new ArgumentException("Length can not be less than 0");
            }
        }
    }

    internal static class NativeViewUtility
    {
        public static NativeView GetWriteView<T>(NativeArray<T> data) where T : struct
        {
            unsafe
            {
                return new NativeView((byte*) data.GetUnsafePtr(), NativeArrayUtility.LengthInBytes(data));
            }
        }

        public static NativeView GetReadView<T>(NativeArray<T> data) where T : struct
        {
            unsafe
            {
                return new NativeView((byte*) data.GetUnsafeReadOnlyPtr(), NativeArrayUtility.LengthInBytes(data));
            }
        }

        public static void WriteByte(NativeView view, int maskIndex, byte value)
        {
            CheckIndex(view, maskIndex);
            unsafe
            {
                *(view.Ptr + maskIndex) = value;
            }
        }

        public static byte ReadByte(NativeView view, int maskIndex)
        {
            CheckIndex(view, maskIndex);
            unsafe
            {
                return *(view.Ptr + maskIndex);
            }
        }

        [Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
        static void CheckIndex(NativeView view, int i)
        {
            if (!(math.asuint(i) < view.LengthInBytes))
            {
                throw new ArgumentException("Can not access memory outside of the obtained view.");
            }
        }

        public static NativeView GetView<T>(ref T value) where T : struct
        {
            var sizeOfT = UnsafeUtility.SizeOf<T>();
            unsafe
            {
                var ptr = (byte*) UnsafeUtility.AddressOf(ref value);
                return new NativeView(ptr, sizeOfT);
            }
        }
    }
}
