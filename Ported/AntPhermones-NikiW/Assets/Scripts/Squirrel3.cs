using System;
using System.Runtime.CompilerServices;
using Unity.Burst;

/// <summary>
///     niki.walker@unity3d.com
///     Better random, ported from: https://github.com/sublee/squirrel3-python/blob/master/squirrel3.py
/// </summary>
public static class Squirrel3
{
    const uint k_Noise1 = 0xb5297a4d;
    const uint k_Noise2 = 0x68e31da4;
    const uint k_Noise3 = 0x1b56c4e9;

    [BurstCompile, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float NextFloat(uint last, uint seed, float min, float max)
    {
        return (float)NextRand(last, seed) / uint.MaxValue * (max - min) + min;
    }

    [BurstCompile, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double NextDouble(uint last, uint seed, float min, float max)
    {
        return (double)NextRand(last, seed) / uint.MaxValue * (max - min) + min;
    }

    [BurstCompile, MethodImpl(MethodImplOptions.AggressiveInlining)]
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
