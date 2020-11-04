using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;

static unsafe public class UnsafeHelper
{
    static public void GetNativeArrayFromBlob<T>(BlobArray<T> blob, out NativeArray<T> output) where T : struct
    {
        void* pPtr = blob.GetUnsafePtr();

        output = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<T>(pPtr, blob.Length, Allocator.None);
    }

    static public void GetNativeArrayFromBlobBuilder<T>(BlobBuilderArray<T> blobBuilder, out NativeArray<T> output) where T : struct
    {
        void* pPtr = blobBuilder.GetUnsafePtr();

        output = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<T>(pPtr, blobBuilder.Length, Allocator.None);
    }
}
