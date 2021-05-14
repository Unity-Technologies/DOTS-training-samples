using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;

using Random = Unity.Mathematics.Random;

public static class OffsetGenerator
{
    public static void CreateRandomOffsets(int width, int height, float minHeight, float maxHeight, Random random, ref DynamicBuffer<OffsetList> buffer)
    {
        int totalSize = width * height;
       
        buffer.ResizeUninitialized(totalSize);
        float rangeHeight = maxHeight - minHeight;
        for (int i = 0; i < totalSize; ++i)
        {
            var randVal = random.NextFloat() * rangeHeight + minHeight;
            buffer[i] = new OffsetList {Value = randVal};
        }
    }
}

