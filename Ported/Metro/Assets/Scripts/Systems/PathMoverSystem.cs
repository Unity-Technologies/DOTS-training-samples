using Unity.Burst;
using Unity.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
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

        var outputDeps = Entities.ForEach((ref Translation translation, ref Rotation rotation, ref PathMoverComponent pathMoverComponent, ref MovementDerivatives movement) =>
        {
            int2 range = pathIndices[pathMoverComponent.m_TrackIndex];
            
            int nextPoint = (pathMoverComponent.CurrentPointIndex + 1) % (range.y - range.x);

            float3 currentPoint = pathData[pathMoverComponent.CurrentPointIndex];
            float3 nextPosition = pathData[nextPoint + range.x];

            float3 forward = math.normalize(nextPosition - currentPoint);
            float3 tangent = math.normalize(math.cross(forward, new float3(0.0f, 1.0f, 0.0f)));
            quaternion nextRotation = quaternion.LookRotation(forward, new float3(0.0f, 1.0f, 0.0f));
            rotation.Value = nextRotation; 

            if (Approach.Apply(ref translation.Value, ref movement.Speed, nextPosition, movement.Acceleration * dt, ARRIVAL_THRESHOLD, FRICTION))
            {
                pathMoverComponent.CurrentPointIndex = nextPoint;
            }
        }).Schedule(inputDeps);
        return outputDeps;
    }
}