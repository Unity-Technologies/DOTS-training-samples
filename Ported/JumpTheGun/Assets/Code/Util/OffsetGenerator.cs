using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;

using Random = Unity.Mathematics.Random;

public static class OffsetGenerator
{
    public static NativeArray<float> CreateRandomOffsets(int width, int height, float minHeight, float maxHeight, Random random, Allocator allocator = Allocator.Persistent)
    {
        int totalSize = width * height;
        
        var outputArray = new NativeArray<float>(totalSize, allocator, NativeArrayOptions.UninitializedMemory);
        float rangeHeight = maxHeight - minHeight;
        for (int i = 0; i < totalSize; ++i)
        {
            var randVal = random.NextFloat() * rangeHeight + minHeight;
            outputArray[i] = randVal;
        }

        return outputArray;
    }
}

