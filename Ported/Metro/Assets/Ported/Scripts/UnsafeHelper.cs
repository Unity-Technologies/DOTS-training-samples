using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;

static unsafe public class UnsafeHelper
{
    static public NativeArray<T> GetNativeArrayFromBlob<T>(BlobArray<T> blob) where T : struct
    {
        void* pPtr = blob.GetUnsafePtr();

        return NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<T>(pPtr, blob.Length, Allocator.None);
    }

    static public NativeArray<T> GetNativeArrayFromBlobBuilder<T>(BlobBuilderArray<T> blobBuilder) where T : struct
    {
        void* pPtr = blobBuilder.GetUnsafePtr();

        return NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<T>(pPtr, blobBuilder.Length, Allocator.None);
    }
}
