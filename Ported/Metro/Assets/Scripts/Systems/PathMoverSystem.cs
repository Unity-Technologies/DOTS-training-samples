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

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var pathData = m_PathPositions;
        var dt = Time.DeltaTime;
        var totalTime = 0.5f;
        var pathIndices = m_PathIndices;

        var outputDeps = Entities.ForEach((Entity entity, int entityInQueryIndex, ref Translation translation, ref PathMoverComponent pathMoverComponent) =>
        {
            int2 range = pathIndices[pathMoverComponent.m_TrackIndex];
            int nextPoint = (pathMoverComponent.CurrentPointIndex + 1) % (range.y - range.x);

            float3 currentPosition = pathData[pathMoverComponent.CurrentPointIndex + range.x];
            float3 nextPosition = pathData[nextPoint + range.x];

            translation.Value = math.lerp(currentPosition, nextPosition, pathMoverComponent.t);
            pathMoverComponent.t += dt / totalTime;

            if (pathMoverComponent.t > 1) 
            {
                pathMoverComponent.CurrentPointIndex = nextPoint;
                pathMoverComponent.t = 0;
            }

        }).Schedule(inputDeps);
        return outputDeps;
    }
}