using Unity.Collections;

public static class OffsetGenerator
{
    public static NativeArray<float> CreateRandomOffsets(int width, int height, float minHeight, float maxHeight)
    {
        int totalSize = width * height;
        var outputArray = new float[totalSize];
        var random = new System.Random();
        float rangeHeight = maxHeight - minHeight;
        for (int i = 0; i < totalSize; ++i)
        {
            var randVal = (float)random.NextDouble() * rangeHeight + minHeight;
            outputArray[i] = randVal;
        }
        return new NativeArray<float>(outputArray, Allocator.Persistent);
    }
}

