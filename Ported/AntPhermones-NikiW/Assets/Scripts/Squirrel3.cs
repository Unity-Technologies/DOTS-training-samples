using System;
using Unity.Burst;

/// <summary>
///     NW: Better random.
///     https://github.com/sublee/squirrel3-python/blob/master/squirrel3.py
/// </summary>
public static class Squirrel3
{
    const uint k_Noise1 = 0xb5297a4d;
    const uint k_Noise2 = 0x68e31da4;
    const uint k_Noise3 = 0x1b56c4e9;

    public static float NextFloat(uint last, uint seed, float min, float max)
    {
        return (float)((double)NextRand(last, seed) / uint.MaxValue * (max - min) + min);
    }
    
    public static uint NextRand(uint n, uint seed = 0)
    {
        n *= k_Noise1;
        n += seed;
        n ^= n >> 8;
        n += k_Noise2;
        n ^= n << 8;
        n *= k_Noise3;
        n ^= n >> 8;
        return n;
    }
}
