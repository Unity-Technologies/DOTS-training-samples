using System;
using System.ComponentModel;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

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
        var segmentPositions = trackSegments.ToComponentDataArray<LocalTransform>(Allocator.TempJob);
        
        var findNearest = new FindNearestNeighborsJob()
        {
            SegmentCount = segmentPositions.Length,
            AllCars = allCarData,
            CarTransforms = allCarTransforms
        };

        state.Dependency = findNearest.Schedule(state.Dependency);
        state.Dependency.Complete();

        //var segmentPositions = trackSegments.ToComponentDataArray<LocalTransform>(Allocator.TempJob);
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
    [Unity.Collections.ReadOnly] public int SegmentCount;
    [Unity.Collections.ReadOnly] public NativeArray<CarData> AllCars;

    [Unity.Collections.ReadOnly] public NativeArray<LocalTransform> CarTransforms;

    private void Execute(Entity entity, ref CarData carData, ref LocalTransform transform)
    {
        //int proposedLane = -1;

        //var car = carData;
        int leftLane = 0, rightLane = 0;
        int segmentCheckingBuffer = 2;

        carData.inFrontCarIndex = -1;
        carData.leftNearestCarIndex = -1;
        carData.rightNearestCarIndex = -1;

        carData.DistanceToCarInFront = float.MaxValue;
        carData.DistanceToCarLeft = float.MaxValue;
        carData.DistanceToCarRight = float.MaxValue;
        
        SegmentCount = SegmentCount > 0 ? SegmentCount : 1;
        for (var i = 0; i < AllCars.Length; ++i)
        {
            var carToCheck = AllCars[i];
            int currentSegmentID = carData.SegmentID;
            int segmentDiff = (carToCheck.SegmentID + SegmentCount) % SegmentCount - currentSegmentID;
            Debug.Log($"Nearest: {carData.Lane}, {carToCheck.Lane}");

            // Only checking the cars in last segment (for side lanes) and cars in front buffer segments 
            if (segmentDiff >= -1 && segmentDiff < segmentCheckingBuffer)
            {
                // Find current lane's front car index && ignore the last segment
                if (carToCheck.Lane == carData.Lane && segmentDiff >= 0)
                {
                    var distanceDiff = math.distance(transform.Position, CarTransforms[i].Position);
                    if (distanceDiff <= carData.DistanceToCarInFront)
                    {
                        carData.DistanceToCarInFront = distanceDiff;
                        carData.inFrontCarIndex = i;
                        carData.CarInFrontSpeed = carToCheck.Speed;
                    }
                }
                
                // Find left lane's nearest car index if applicable
                leftLane = carData.Lane + 1;
                if (leftLane < 4 && carToCheck.Lane == leftLane)
                {
                    var carPositionShiftLeft = transform.Position;
                    var distanceDiff = math.distance(carPositionShiftLeft, CarTransforms[i].Position);
                    if (distanceDiff <= carData.DistanceToCarLeft)
                    {
                        carData.DistanceToCarLeft = distanceDiff;
                        carData.leftNearestCarIndex = i;
                    }
                }

                // Find right lane's nearest car index if applicable
                rightLane = carData.Lane - 1;
                if (rightLane > -1 && carToCheck.Lane == rightLane)
                {
                    var carPositionShiftRight = transform.Position;
                    var distanceDiff = math.distance(carPositionShiftRight, CarTransforms[i].Position);
                    if (distanceDiff <= carData.DistanceToCarRight)
                    {
                        carData.DistanceToCarRight = distanceDiff;
                        carData.rightNearestCarIndex = i;
                    }
                }
            }
        }

        // // Front car index is already assigned
        // if(car.inFrontCarIndex > -1)
        // {
        //     if (AllCars[car.inFrontCarIndex].Speed == car.Speed)
        //     {
        //         proposedLane = carData.Lane < 3 ? carData.TargetLane = carData.Lane + 1 : 2;
        //     }
        //     else
        //     {
        //         carData.inFrontCarIndex = -1;
        //     }
        // }
        // // Front car index not yet assigned
        // else
        // {
        //     for (int j = 0; j < AllCars.Length; ++j)
        //     {
        //         var carToCheck = AllCars[j];
        //
        //         if (car.Lane != carToCheck.Lane)
        //             continue;
        //
        //         if (car.Speed <= carToCheck.Speed)
        //             continue;
        //
        //         var carToCheckSegment = car.SegmentID == 55 ? carToCheck.SegmentID == 0 ? 56 : carToCheck.SegmentID : carToCheck.SegmentID;
        //
        //         if (carToCheckSegment < car.SegmentID)
        //             continue;
        //
        //         var distanceCheck = math.distance(transform.Position, CarTransforms[j].Position);
        //
        //         if (distanceCheck < 5)
        //         {
        //             carData.inFrontCarIndex = j;
        //             carData.Speed = carToCheck.Speed;
        //             proposedLane = carData.Lane < 3 ? carData.Lane + 1 : 2;
        //             break;
        //         }
        //     }
        // }
        //
        // if (proposedLane == -1)
        // {
        //     if(car.inFrontCarIndex == -1)
        //     {
        //         proposedLane = carData.Lane > 0 ? carData.Lane - 1 : -1;
        //
        //         if (proposedLane == -1)
        //             return;
        //
        //     } else
        //     {
        //         return;
        //     }            
        // }
        //     
        //
        // int carsInRange = 0;
        //
        // for (int j = 0; j < AllCars.Length; ++j)
        // {
        //     var carToCheck = AllCars[j];
        //
        //     if (carToCheck.Lane == proposedLane)
        //     {
        //         var distanceCheck = math.distancesq(transform.Position, CarTransforms[j].Position);
        //
        //         //Overtake distance 
        //         if (distanceCheck < 50)
        //         {
        //             carsInRange = 1;
        //             break;
        //         }
        //     }            
        // }
        //
        // if (carsInRange > 0)
        //     return;
        //
        // carData.TargetLane = proposedLane;
        // carData.inFrontCarIndex = -1;

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
        // // Update speed.
        // if (carData.Lane != carData.TargetLane)
        // {
        //     carData.Lane = carData.TargetLane;
        //     carData.Speed = carData.DefaultSpeed;
        // }
        
        // Update lane and speed
        // if no car in front buffer segments, keep in the current lane, but boost the speed to overtake speed
        if (carData.inFrontCarIndex < 0)
        {
            carData.Speed = carData.OvertakeSpeed;
        }
        // Has car in front buffer segments
        else
        {
            // Only if the distance diff in lane smaller than the safeInLane distance threshold, check if speed or lane needs change 
            if (carData.DistanceToCarInFront <= carData.SafeDistanceInFront)
            {
                bool canMergeLeft = carData.Lane < 3 && carData.DistanceToCarLeft > carData.SafeDistanceOnSide;
                bool canMergeRight = carData.Lane > 0 && carData.DistanceToCarRight > carData.SafeDistanceOnSide;

                // Try merge left first
                if (canMergeLeft)
                {
                    carData.Lane += 1;
                    carData.Speed = carData.OvertakeSpeed;
                }
                // Then try merge right
                else if (canMergeRight)
                {
                    carData.Lane -= 1;
                    carData.Speed = carData.OvertakeSpeed;
                }
                // Cannot merge either way, keep the same speed as the front car, stay in lane
                else
                {
                    carData.Speed = carData.CarInFrontSpeed;
                }
            }
        }
        

        float carSpeed = carData.Speed * DeltaTime;

        float carLane = 4f + (2.45f * carData.Lane);
        //Debug.Log($"After check: {carData.Lane}, currentDist: {carLane}");
        
        

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
        


        //Calculate rotation
        var calculatePrecentage = 1 - math.clamp((currentDistance - 1f) / (carData.SegmentDistance - 1f), 0, 1);
        carData.LerpDistance = calculatePrecentage;
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