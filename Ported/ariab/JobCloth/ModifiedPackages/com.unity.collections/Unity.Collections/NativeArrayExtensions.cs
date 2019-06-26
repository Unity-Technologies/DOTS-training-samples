using System;
using Unity.Collections.LowLevel.Unsafe;

namespace Unity.Collections
{
    unsafe static public class NativeArrayExtensions
    {
        public static bool Contains<T, U>(this NativeArray<T> array, U value) where T : struct, IEquatable<U>
        {
            return IndexOf<T, U>(array.GetUnsafeReadOnlyPtr(), array.Length, value) != -1;
        }

        public static int IndexOf<T, U>(this NativeArray<T> array, U value) where T : struct, IEquatable<U>
        {
            return IndexOf<T, U>(array.GetUnsafeReadOnlyPtr(), array.Length, value);
        }

        public static bool Contains<T, U>(this NativeList<T> array, U value) where T : struct, IEquatable<U>
        {
            return IndexOf<T,U>(array.GetUnsafePtr(), array.Length, value) != -1;
        }

        public static int IndexOf<T, U>(this NativeList<T> array, U value) where T : struct, IEquatable<U>
        {
            return IndexOf<T, U>(array.GetUnsafePtr(), array.Length, value);
        }

        static int IndexOf<T, U>(void* ptr, int size, U value) where T : struct, IEquatable<U>
        {
            for (int i = 0; i != size; i++)
            {
                if (UnsafeUtility.ReadArrayElement<T>(ptr, i).Equals(value))
                    return i;
            }
            return -1;
        }

        /// <summary>
        /// Reinterpret a native array as being of another type, aliasing its contents via type punning.
        /// </summary>
        /// <param name="array">The array to alias</param>
        /// <typeparam name="T">Source type of array elements</typeparam>
        /// <typeparam name="U">Target type of array elements</typeparam>
        /// <returns>The same array, with a different type of element</returns>
        public static NativeArray<U> Reinterpret<T, U>(this NativeArray<T> array) where U : struct where T : struct
        {
            var tSize = UnsafeUtility.SizeOf<T>();
            var uSize = UnsafeUtility.SizeOf<U>();

            var byteLen = ((long) array.Length) * tSize;
            var uLen = byteLen / uSize;

#if ENABLE_UNITY_COLLECTIONS_CHECKS
            if (uLen * uSize != byteLen)
            {
                throw new InvalidOperationException($"Types {typeof(T)} (array length {array.Length}) and {typeof(U)} cannot be aliased due to size constraints. The size of the types and lengths involved must line up.");
            }

#endif
            var ptr = NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks(array);
            var result = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<U>(ptr, (int) uLen, Allocator.Invalid);

#if ENABLE_UNITY_COLLECTIONS_CHECKS
            var handle = NativeArrayUnsafeUtility.GetAtomicSafetyHandle(array);
            NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref result, handle);
#endif

            return result;
        }
    }
}
