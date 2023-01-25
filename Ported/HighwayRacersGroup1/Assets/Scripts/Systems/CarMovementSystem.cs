using System;
using System.ComponentModel;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
partial struct CarMovementSystem : ISystem
{
    private EntityQuery trackSegments;
    
    public void OnCreate(ref SystemState state)
    {
        trackSegments =
            state.GetEntityQuery(ComponentType.ReadOnly<Segment>(), ComponentType.ReadOnly<LocalTransform>());
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var segmentPositions = trackSegments.ToComponentDataArray<LocalTransform>(Allocator.TempJob);
        var segmentDirections = trackSegments.ToComponentDataArray<Segment>(Allocator.TempJob);
        var carMovementJob = new CarMovementJob
        {
            Segments = segmentPositions,
            SegmentDirections = segmentDirections,
            DeltaTime = SystemAPI.Time.DeltaTime
        };

        carMovementJob.ScheduleParallel();
    }
}

[BurstCompile]
partial struct CarMovementJob : IJobEntity
{
    [Unity.Collections.ReadOnly]
    public NativeArray<LocalTransform> Segments;

    [Unity.Collections.ReadOnly]
    public NativeArray<Segment> SegmentDirections;
    public float DeltaTime;

    private void Execute(Entity entity, ref CarData carData, ref LocalTransform transform)
    {
        float carLane = 4f + (2.45f * carData.Lane);

        var targetSegmentIndex = carData.SegmentID + 1 < Segments.Length ? carData.SegmentID + 1 : 0;

        var targetSegmentPosition = Segments[targetSegmentIndex];
        var targetSegmentRight = SegmentDirections[targetSegmentIndex].Right;
        var targetPosition = targetSegmentPosition.Position + (targetSegmentRight * -carLane);;

        var currentDistance = math.distance(transform.Position, targetPosition);
        
        transform.Position = MoveTowards(transform.Position, targetPosition, 10f * DeltaTime);
        
        if (currentDistance <= 0.025f)
        {
            carData.SegmentID = carData.SegmentID < Segments.Length - 1 ? carData.SegmentID + 1 : 0;
        }
    }
    
    public static float3 MoveTowards(float3 current, float3 target, float maxDistanceDelta)
    {
        float deltaX = target.x - current.x;
        float deltaY = target.y - current.y;
        float deltaZ = target.z - current.z;
        float sqdist = deltaX * deltaX + deltaY * deltaY  + deltaZ * deltaZ ;
        if (sqdist == 0 || sqdist <= maxDistanceDelta * maxDistanceDelta)
            return target;
        var dist = (float)math.sqrt(sqdist);
        return new float3(current.x + deltaX / dist * maxDistanceDelta,
            current.y + deltaY / dist * maxDistanceDelta,
            current.z + deltaZ / dist * maxDistanceDelta);
    }
}