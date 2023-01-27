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

    private NativeArray<LocalTransform> segmentPositions;
    private NativeArray<Segment> segmentDirections;

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
        //var allCarData = cars.ToComponentDataArray<CarData>(Allocator.TempJob);
        //var allCarTransforms = cars.ToComponentDataArray<LocalTransform>(Allocator.TempJob);
        
        if(segmentPositions.Length < 1)
            segmentPositions = trackSegments.ToComponentDataArray<LocalTransform>(Allocator.Persistent);

        if(segmentDirections.Length < 1)
            segmentDirections = trackSegments.ToComponentDataArray<Segment>(Allocator.Persistent);

        //var segmentPositions = trackSegments.ToComponentDataArray<LocalTransform>(Allocator.TempJob);
        //var segmentDirections = trackSegments.ToComponentDataArray<Segment>(Allocator.TempJob);


        var SegmentCars = new NativeArray<CarData>(segmentPositions.Length * 10, Allocator.TempJob);

        var findJob = new FindNearestJob
        {
            SegmentCars = SegmentCars
        };

        state.Dependency = findJob.Schedule(state.Dependency);
        state.Dependency.Complete();

        var carMovementJob = new CarMovementJob
        {
            SegmentCars = SegmentCars,
            Segments = segmentPositions,
            SegmentDirections = segmentDirections,
            DeltaTime = SystemAPI.Time.DeltaTime
        };

        state.Dependency = carMovementJob.ScheduleParallel(state.Dependency);
        
    }
}


[BurstCompile]
partial struct FindNearestJob : IJobEntity
{
    public NativeArray<CarData> SegmentCars;

    private void Execute(Entity entity, ref CarData carData)
    {
        int segmentIndex = carData.SegmentID * 10;
       
        for(int i = 0; i < 10; i++)
        {
            var currentSegmentCarID = SegmentCars[segmentIndex];

            if(currentSegmentCarID.Speed < 1)
            {
                break;
            }

            segmentIndex++;
        }

        SegmentCars[segmentIndex] = carData;
        
    }
}


[BurstCompile]
partial struct CarMovementJob : IJobEntity
{
    [Unity.Collections.ReadOnly] public NativeArray<LocalTransform> Segments;

    [Unity.Collections.ReadOnly] public NativeArray<Segment> SegmentDirections;

    [Unity.Collections.ReadOnly] public NativeArray<CarData> SegmentCars;

    public float DeltaTime;

    private void Execute(Entity entity, ref CarData carData, ref LocalTransform transform)
    {
        int currentSegmentID = carData.SegmentID * 10;
        int nextSegmentID = carData.SegmentID + 1 > Segments.Length - 1 ? 0 : (carData.SegmentID + 1) * 10;
        int previousSegmentID = carData.SegmentID + 2 > Segments.Length - 1 ? 0 : (carData.SegmentID + 2) * 10;
        int previousSegmentID2 = carData.SegmentID - 1 < 0 ? (Segments.Length - 1) * 10 : (carData.SegmentID - 1) * 10;
        //int previousSegmentID = carData.SegmentID - 1 < 0 ? (Segments.Length - 1) * 10 : (carData.SegmentID - 1) * 10;

        int fastLane = carData.Lane < 3 ? carData.Lane + 1 : 3;
        int slowLane = carData.Lane > 0 ? carData.Lane - 1 : 0;
        int pass = 0;

        var car = carData;

        for (int j = 0; j < 40; j++)
        {
            var carToCheck = SegmentCars[0];

            if (j < 10)
            {
                carToCheck = SegmentCars[currentSegmentID + j];
            }
            else if (j > 9 && j < 20)
            {
                carToCheck = SegmentCars[nextSegmentID + (j - 10)];
            }            
            else if (j > 19 && j < 30)
            {
                carToCheck = SegmentCars[previousSegmentID + (j - 20)];
            }
            else if (j > 29 && j < 40)
            {
                carToCheck = SegmentCars[previousSegmentID2 + (j - 30)];
            }

            if (carToCheck.Speed > 1 && carToCheck.CarID != carData.CarID)
            {
                if (car.Lane == carToCheck.Lane)
                {
                    if (car.Speed <= carToCheck.Speed)
                        continue;

                    var carToCheckSegment = carData.SegmentID == Segments.Length - 1 ? carToCheck.SegmentID == 0 ? Segments.Length : carToCheck.SegmentID : carToCheck.SegmentID;

                    if (carToCheckSegment < carData.SegmentID)
                        continue;

                    if (carToCheck.LerpDistance < carData.LerpDistance)
                        continue;

                    var distanceCheck = math.distance(transform.Position, carToCheck.Position);

                    if (distanceCheck < 6)
                    {                       
                        carData.Speed = carToCheck.Speed;
                        pass = 1;
                    }
                }  
                else if (carToCheck.Lane == fastLane)
                {
                    var distanceCheck = math.distance(transform.Position, carToCheck.Position);

                    if (distanceCheck < 10)
                    {
                        fastLane = -1;
                        pass = 0;
                    }
                }           
                else if (carToCheck.Lane == slowLane)
                {
                    var distanceCheck = math.distance(transform.Position, carToCheck.Position);

                    if (distanceCheck < 10)
                    {
                        slowLane = -1;
                        pass = 0;
                    }
                }
            }            
        }
        
        if(pass == 1 || carData.Speed < carData.DefaultSpeed)
        {
            if(fastLane > -1)
            {
                if (carData.Lane != fastLane)
                {
                    carData.Lane = fastLane;
                    carData.Speed = carData.DefaultSpeed;
                }
            } 
            else if(slowLane > -1)
            {
                if (carData.Lane != slowLane)
                {
                    carData.Lane = slowLane;
                    carData.Speed = carData.DefaultSpeed;
                }
            }            
        }
        /*
        if (carData.Lane != carData.TargetLane)
        {
            carData.Lane = carData.TargetLane;
        }
        */

        float carSpeed = carData.Speed * DeltaTime;

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
        carData.Position = transform.Position;
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