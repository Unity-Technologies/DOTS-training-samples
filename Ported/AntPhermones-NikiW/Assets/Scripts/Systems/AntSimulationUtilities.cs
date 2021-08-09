using System;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public static class AntSimulationUtilities
{
    // NWALKER: Trying out in keyword to improve perf.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [BurstCompile]
    public static bool2 CalculateIsInBounds(in int x, in int y, in int size, out int index)
    {
        index = x + y * size;
        return new bool2(x >= 0 && x < size, y >= 0 && y < size);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [BurstCompile]
    public static bool2 CalculateIsInBounds(in float2 pos, in int size, out int index)
    {
        var x = Mathf.RoundToInt(pos.x);
        var y = Mathf.RoundToInt(pos.y);
        index = x + y * size;
        return new bool2(x >= 0 && x < size, y >= 0 && y < size);
    }

    public static DynamicBuffer<T> GetSingletonBuffer<T>(this SystemBase system) where T : struct, IBufferElementData
    {
        var entity = system.GetSingletonEntity<T>();
        return system.GetBuffer<T>(entity);
    }

    public static uint GeneratePerFrameRandomSeed()
    {
        return (uint)(UnityEngine.Random.value * uint.MaxValue);
    }
}
