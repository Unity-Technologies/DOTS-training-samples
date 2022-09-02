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

    // a and b must be [0 - 180]
    public static float3 GetRandomInArcSphere(float innerDegrees, float outerDegrees, float3 aroundAxis, ref Unity.Mathematics.Random random)
    {
        var v = random.NextFloat(0f, 1f);
        var a = math.cos(math.radians(innerDegrees));
        var b = math.cos(math.radians(outerDegrees));

        float azimuth = v * 2f * math.PI;
        float cosDistFromZenith = random.NextFloat(math.min(a, b), math.max(a, b));
        float sinDistFromZenith = math.sqrt(1f - cosDistFromZenith * cosDistFromZenith);
        float3 pqr = new float3(math.cos(azimuth) * sinDistFromZenith, math.sin(azimuth) * sinDistFromZenith, cosDistFromZenith);
        float3 pAxis = math.abs(aroundAxis.x) < 0.9f ? math.right() : math.up();
        float3 qAxis = math.normalizesafe(math.cross(aroundAxis, pAxis));
        pAxis = math.cross(qAxis, aroundAxis);
        float3 position = (pqr.x * pAxis) + (pqr.y * qAxis) + (pqr.z * aroundAxis);
        return position;
    }


    public static void DropResource(Entity resourceEntity, Entity carrierEntity, EntityCommandBuffer.ParallelWriter ecbParallel, int sortKey)
    {
        ecbParallel.RemoveComponent<ResourceCarrier>(sortKey, resourceEntity);
        ecbParallel.RemoveComponent<BeeTargetResource>(sortKey, carrierEntity);
    }
}