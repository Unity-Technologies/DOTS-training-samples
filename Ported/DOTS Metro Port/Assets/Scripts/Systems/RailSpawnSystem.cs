using System.Diagnostics;
using System.Resources;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
partial struct RailSpawnSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
    }

    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<Config>();

        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        foreach (var buffer in SystemAPI.Query<DynamicBuffer<BezierPoint>>())
        {
            var array = buffer.AsNativeArray();
            BezierPath.MeasurePath(array);
            float pathLength = BezierPath.Get_PathLength(array);

            float currentDistance = 0f;
            while (currentDistance < pathLength)
            {
                float distAsPercentage = currentDistance / pathLength;
                float3 posOnRail = BezierPath.Get_Position(array, distAsPercentage);
                float3 tangentOnRail = BezierPath.Get_NormalAtPosition(array, distAsPercentage);

                var rotation = Quaternion.LookRotation(tangentOnRail);

                var rail = ecb.Instantiate(config.RailPrefab);
                ecb.SetComponent(rail, new Translation { Value = posOnRail });
                ecb.SetComponent(rail, new Rotation { Value = rotation });

                currentDistance += 1.0f;
            }
        }

        // This system should only run once at startup. So it disables itself after one update.
        state.Enabled = false;
    }
}