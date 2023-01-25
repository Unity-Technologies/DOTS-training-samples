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
        float carSpeed = 50 * DeltaTime;
        float carLane = 4f + (2.45f * carData.Lane);

        var currentSegmentIndex = carData.SegmentID;
        var currentSegmentPosition = Segments[currentSegmentIndex];
        var currentSegmentRight = SegmentDirections[currentSegmentIndex].Right;
        var currentPosition = currentSegmentPosition.Position + (currentSegmentRight * -carLane); ;


        var targetSegmentIndex = carData.SegmentID + 1 < Segments.Length ? carData.SegmentID + 1 : 0;
        var targetSegmentPosition = Segments[targetSegmentIndex];
        var targetSegmentRight = SegmentDirections[targetSegmentIndex].Right;
        var targetPosition = targetSegmentPosition.Position + (targetSegmentRight * -carLane);

        var futureSegmentIndex = carData.SegmentID + 2 < Segments.Length ? carData.SegmentID + 2 : 2 - (Segments.Length - carData.SegmentID);
        var futureSegmentPosition = Segments[futureSegmentIndex];
        var futureSegmentRight = SegmentDirections[futureSegmentIndex].Right;
        var futurePosition = futureSegmentPosition.Position + (futureSegmentRight * -carLane);

        var currentDistance = math.distance(transform.Position, targetPosition);

        //Calculate rotation
        var calculatePrecentage = 1 - math.clamp((currentDistance - 1f) / (carData.SegmentDistance - 1f), 0 , 1);
        float3 currentDirection = targetPosition - currentPosition;
        float3 targetDirection = futurePosition - targetPosition;
        float3 directionLerp = math.normalize(new float3(math.lerp(currentDirection.x, targetDirection.x, calculatePrecentage), 0, math.lerp(currentDirection.z, targetDirection.z, calculatePrecentage))) * carSpeed;
        float3 directionUp = new float3(0, 1, 0);
        quaternion carRotation = quaternion.LookRotation(directionLerp, directionUp);

        transform.Position = MoveTowards(transform.Position, targetPosition, carSpeed);

        transform.Rotation = carRotation;

        if (currentDistance <= 1f)
        {
            carData.SegmentID = carData.SegmentID < Segments.Length - 1 ? carData.SegmentID + 1 : 0;
            carData.SegmentDistance = math.distance(transform.Position, futurePosition);
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