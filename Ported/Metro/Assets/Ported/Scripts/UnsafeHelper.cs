using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;

static unsafe public class UnsafeHelper
{
    static public NativeArray<T> GetNativeArrayFromBlob<T>(ref BlobArray<T> blob) where T : struct
    {
        void* pPtr = blob.GetUnsafePtr();

        var output = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<T>(pPtr, blob.Length, Allocator.None);

        NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref output, AtomicSafetyHandle.GetTempMemoryHandle());

        return output;
    }

    static public NativeArray<T> GetNativeArrayFromBlobBuilder<T>(ref BlobBuilderArray<T> blobBuilder) where T : struct
    {
        void* pPtr = blobBuilder.GetUnsafePtr();

        var output = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<T>(pPtr, blobBuilder.Length, Allocator.None);

        NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref output, AtomicSafetyHandle.GetTempMemoryHandle());

        return output;
    }
}
