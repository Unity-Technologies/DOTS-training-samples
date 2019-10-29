using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace Unity.Entities.Determinism
{
    internal static class Spooky
    {
        // use the native implementation from dots (not burstable)
        public static Hash128 Hash<T>(NativeArray<T> data) where T : struct
        {
            if( NativeArrayUtility.IsInvalidOrEmpty(data) ) return new Hash128();

            var sizeOfT = UnsafeUtility.SizeOf<T>();
            var sizeInBytes = sizeOfT * data.Length;

            var hash = new Hash128();
            unsafe
            {
                var ptr = data.GetUnsafeReadOnlyPtr();
                var resultPtr = (UnityEngine.Hash128*) UnsafeUtility.AddressOf(ref hash);
                HashUnsafeUtilities.ComputeHash128(ptr, (ulong)sizeInBytes, resultPtr);
            }

            return hash;
        }
    }
}