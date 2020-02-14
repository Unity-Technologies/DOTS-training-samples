using Unity.Burst;
using Unity.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using static Unity.Mathematics.math;
using UnityEngine;

public class PathMoverSystem : JobComponentSystem
{
    public NativeArray<float3> m_PathPositions;
    public NativeArray<int2> m_PathIndices;

    public const float FRICTION = 0.8f;
    public const float ARRIVAL_THRESHOLD = 0.02f;

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var pathData = m_PathPositions;
        var dt = Time.DeltaTime;
        var pathIndices = m_PathIndices;

        var outputDeps = Entities.ForEach((ref Translation translation, ref PathMoverComponent pathMoverComponent, ref MovementDerivatives movement) =>
        {
            int2 range = pathIndices[pathMoverComponent.m_TrackIndex];
            int nextPoint = (pathMoverComponent.CurrentPointIndex + 1) % (range.y - range.x);

            float3 nextPosition = pathData[nextPoint + range.x];

            if (Approach.Apply(ref translation.Value, ref movement.Speed, nextPosition, movement.Acceleration * dt, ARRIVAL_THRESHOLD, FRICTION))
            {
                pathMoverComponent.CurrentPointIndex = nextPoint;
            }
        }).Schedule(inputDeps);
        return outputDeps;
    }
}