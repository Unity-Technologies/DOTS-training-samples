using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

static unsafe public class UnsafeHelper
{
    static unsafe public NativeArray<T> GetNativeArrayFromBlob<T>(BlobArray<T> blob)
    {
        unsafe
        {
            T* pPtr = blob.GetUnsafePtr();

            return NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<T>(pPtr, blob.Length, Allocator.None);
        }
    }

    static unsafe public NativeArray<T> GetNativeArrayFromBlobBuilder<T>(BlobBuilderArray<T> blobBuilder)
    {
        unsafe
        {
            T* pPtr = blobBuilder.GetUnsafePtr();

            return NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<T>(pPtr, blob.Length, Allocator.None);
        }
    }
}
