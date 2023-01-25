using System;
using System.ComponentModel;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
partial struct CarMovementSystem : ISystem
{
    private EntityQuery trackSegments;
    private EntityQuery cars;

    public void OnCreate(ref SystemState state)
    {
        trackSegments =
            state.GetEntityQuery(ComponentType.ReadOnly<Segment>(), ComponentType.ReadOnly<LocalTransform>());

        cars = state.GetEntityQuery(ComponentType.ReadOnly<CarData>(), ComponentType.ReadOnly<LocalTransform>());
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // todo: find nearest neighbors
        var allCarData = cars.ToComponentDataArray<CarData>(Allocator.TempJob);
        var allCarTransforms = cars.ToComponentDataArray<LocalTransform>(Allocator.TempJob);

        var findNearest = new FindNearestNeighborsJob()
        {
            AllCars = allCarData,
            CarTransforms = allCarTransforms
        };

        state.Dependency = findNearest.Schedule(state.Dependency);
        state.Dependency.Complete();

        var segmentPositions = trackSegments.ToComponentDataArray<LocalTransform>(Allocator.TempJob);
        var segmentDirections = trackSegments.ToComponentDataArray<Segment>(Allocator.TempJob);
        var carMovementJob = new CarMovementJob
        {
            Segments = segmentPositions,
            SegmentDirections = segmentDirections,
            DeltaTime = SystemAPI.Time.DeltaTime
        };

        state.Dependency = carMovementJob.ScheduleParallel(state.Dependency);
    }
}

[BurstCompile]
partial struct FindNearestNeighborsJob : IJobEntity
{
    [Unity.Collections.ReadOnly] public NativeArray<CarData> AllCars;

    [Unity.Collections.ReadOnly] public NativeArray<LocalTransform> CarTransforms;

    private void Execute(Entity entity, ref CarData carData, ref LocalTransform transform)
    {
        float minDistance = float.MaxValue;
        float maxSpeed = carData.Speed;
        for (var i = 0; i < AllCars.Length; ++i)
        {
            var carToCheck = AllCars[i];
            if (carData.Lane != carToCheck.Lane)
                continue;

            
            
            var position = CarTransforms[i].Position;
            minDistance = math.min(minDistance, math.distancesq(transform.Position, position));
            maxSpeed = carToCheck.Speed;
        }

        carData.DistanceToCarInFront = minDistance;
        carData.CarInFrontSpeed = maxSpeed;
    }
}

[BurstCompile]
partial struct CarMovementJob : IJobEntity
{
    [Unity.Collections.ReadOnly] public NativeArray<LocalTransform> Segments;

    [Unity.Collections.ReadOnly] public NativeArray<Segment> SegmentDirections;

    public float DeltaTime;

    private void Execute(Entity entity, ref CarData carData, ref LocalTransform transform)
    {
        float carLane = 4f + (2.45f * carData.Lane);

        var currentSegmentIndex = carData.SegmentID;
        var currentSegmentPosition = Segments[currentSegmentIndex];
        var currentSegmentRight = SegmentDirections[currentSegmentIndex].Right;
        var currentPosition = currentSegmentPosition.Position + (currentSegmentRight * -carLane);

        var targetSegmentIndex = carData.SegmentID + 1 < Segments.Length ? carData.SegmentID + 1 : 0;
        var targetSegmentPosition = Segments[targetSegmentIndex];
        var targetSegmentRight = SegmentDirections[targetSegmentIndex].Right;
        var targetPosition = targetSegmentPosition.Position + (targetSegmentRight * -carLane);

        var futureSegmentIndex = carData.SegmentID + 2 < Segments.Length
            ? carData.SegmentID + 2
            : 2 - (Segments.Length - carData.SegmentID);
        var futureSegmentPosition = Segments[futureSegmentIndex];
        var futureSegmentRight = SegmentDirections[futureSegmentIndex].Right;
        var futurePosition = futureSegmentPosition.Position + (futureSegmentRight * -carLane);

        var currentDistance = math.distance(transform.Position, targetPosition);
        
        // Update speed.
        if (carData.DistanceToCarInFront < 3f)
        {
            carData.Speed = carData.CarInFrontSpeed;
        }

        float carSpeed = carData.Speed * DeltaTime;

        //Calculate rotation
        var calculatePrecentage = 1 - math.clamp((currentDistance - 1f) / (carData.SegmentDistance - 1f), 0, 1);
        float3 currentDirection = targetPosition - currentPosition;
        float3 targetDirection = futurePosition - targetPosition;
        float3 directionLerp =
            math.normalize(new float3(math.lerp(currentDirection.x, targetDirection.x, calculatePrecentage), 0,
                math.lerp(currentDirection.z, targetDirection.z, calculatePrecentage))) * carSpeed;
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
        float sqdist = deltaX * deltaX + deltaY * deltaY + deltaZ * deltaZ;
        if (sqdist == 0 || sqdist <= maxDistanceDelta * maxDistanceDelta)
            return target;
        var dist = (float)math.sqrt(sqdist);
        return new float3(current.x + deltaX / dist * maxDistanceDelta,
            current.y + deltaY / dist * maxDistanceDelta,
            current.z + deltaZ / dist * maxDistanceDelta);
    }
}