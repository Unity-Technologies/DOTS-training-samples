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
            
            int nextPoint = (pathMoverComponent.CurrentPointIndex + 1 - range.x) % (range.y - range.x) + range.x;

            float3 currentPosition = pathData[pathMoverComponent.CurrentPointIndex];
            float3 nextPosition = pathData[nextPoint];

            // // Hacking in scalar speed for now:
            movement.Speed += math.length(movement.Acceleration) * dt;
            float movementSpeedScalar = math.length(movement.Speed);

            float3 segmentDirection = nextPosition - currentPosition;
            float segmentLength = math.length(segmentDirection);
            // // segmentDirection /= segmentLength;

            float3 segmentV0Offset = translation.Value - currentPosition;
            float segmentV0OffsetLength = math.dot(segmentV0Offset, segmentV0Offset) > 1e-3f ? math.length(segmentV0Offset) : 0.0f;
            segmentV0OffsetLength += movementSpeedScalar * dt;

            float t = math.saturate(segmentV0OffsetLength / segmentLength);
            translation.Value = math.lerp(currentPosition, nextPosition, t);
            if (t > 0.999f)
            {
                pathMoverComponent.CurrentPointIndex = nextPoint;
                translation.Value = nextPosition;
            }

            float3 forward = math.normalize(nextPosition - currentPosition);
            float3 tangent = math.normalize(math.cross(forward, new float3(0.0f, 1.0f, 0.0f)));
            quaternion nextRotation = quaternion.LookRotation(forward, new float3(0.0f, 1.0f, 0.0f));
            rotation.Value = nextRotation;

            // if (Approach.Apply(ref translation.Value, ref movement.Speed, nextPosition, movement.Acceleration * dt, ARRIVAL_THRESHOLD, FRICTION))
            // {
            //     pathMoverComponent.CurrentPointIndex = nextPoint;
            // }
        }).Schedule(inputDeps);
        return outputDeps;
    }
}