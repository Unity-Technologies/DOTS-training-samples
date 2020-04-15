using System;
using Unity.Collections;

/// <summary>
/// Random for Burst compiled jobs
/// </summary>
public struct FloatRandom : IDisposable
{
    private const int k_NumberCount = 1000;

    public NativeArray<float> Values; // index 0 is the current index pointer

    public static FloatRandom Create(int seed, int numberCount = k_NumberCount)
    {
        if (seed == 0)
        {
            seed = UnityEngine.Random.Range(0, int.MaxValue);
        }

        var floatRand = new FloatRandom();
        var random = new System.Random(seed);
        floatRand.Values = new NativeArray<float>(numberCount + 1, Allocator.Persistent);
        for (var i = 1; i <= numberCount; i++)
        {
            floatRand.Values[i] = (float)random.NextDouble();
        }

        floatRand.Values[0] = 0;
        return floatRand;
    }

    public float NextFloat()
    {
        Values[0]++;
        if (Values[0] >= Values.Length)
        {
            Values[0] = 1;
        }

        return Values[(int)Values[0]];
    }

    public void Dispose()
    {
        if (Values.IsCreated)
        {
            Values.Dispose();
        }
    }
}
