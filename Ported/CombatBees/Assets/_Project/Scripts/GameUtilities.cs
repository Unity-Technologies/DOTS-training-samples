using System;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public static class GameUtilities
{
    public static float4 ColorToFloat4(Color color)
    {
        return new float4
        {
            x = color.r,
            y = color.g,
            z = color.b,
            w = color.a,
        };
    }
    
    public static float4 ColorToFloat4(Color color, float alphaOverride)
    {
        return new float4
        {
            x = color.r,
            y = color.g,
            z = color.b,
            w = alphaOverride,
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float3 ProjectOnPlane(float3 vector, float3 onPlaneNormal)
    {
        return vector - math.projectsafe(vector, onPlaneNormal);
    }

    public static int FindCellIndexOfPosition(float3 position, float3 gridBottomCorner)
    {
        return default;
    }

    public static float3 GetRandomPosInsideBox(ref Unity.Mathematics.Random random, Box box)
    {
        return new float3
        {
            x = random.NextFloat(box.Center.x - box.Extents.x, box.Center.x + box.Extents.x),
            y = random.NextFloat(box.Center.y - box.Extents.y, box.Center.y + box.Extents.y),
            z = random.NextFloat(box.Center.z - box.Extents.z, box.Center.z + box.Extents.z),
        };
    }
}