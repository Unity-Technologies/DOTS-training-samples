using Aspects;
using Authoring;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Burst;
using Unity.Collections;
using Unity.Transforms;
using System.Collections.Generic;

[BurstCompile]
public partial struct CarMovement : ISystem
{
    private EntityQuery laneQuery;
    EntityQuery carQuery;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Config>();
        laneQuery = SystemAPI.QueryBuilder().WithAll<Lane>().Build();
        state.RequireForUpdate(laneQuery);
        carQuery = SystemAPI.QueryBuilder().WithAll<Car>().Build();
        state.RequireForUpdate(carQuery);
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<Config>();
        // Get array of of lanes
        var lanes = laneQuery.ToComponentDataArray<Lane>(Allocator.Temp);

        Unity.Collections.NativeArray<Car> allCars = carQuery.ToComponentDataArray<Car>(Allocator.Temp);

        foreach (var car in SystemAPI.Query<CarAspect>())
        {
            // if (car.Distance > 100.0)
            // {
            //     car.Distance = 0.0f;
            // }
            //get car in front of us
            Car neighbor = allCars[0];//assume there is a car and that we will have one close to us
            float neighborDelta = 1000.0f;

            int rightCount = 0;
            int leftCount = 0;

            bool leftLaneOK = true;//set to false if we are at the leftmost lane
            bool rightLaneOK = true;//set to false if we are at the rightmost lane

            foreach (var other in allCars)
            {
                if(other.Index == car.Index) { continue; }

                float min = car.Distance - config.LaneChangeClearance;
                float max = car.Distance + config.LaneChangeClearance;

                int leftLane = car.LaneNumber + 1;
                int rightLane = car.LaneNumber - 1;

                if(other.LaneNumber == leftLane && other.Distance > min && other.Distance < max)
                {
                    leftLaneOK = false;
                }

                if(other.LaneNumber == rightLane && other.Distance > min && other.Distance < max)
                {
                    rightLaneOK = false;
                }

                if(other.LaneNumber != car.LaneNumber) { continue; }

                float delta = other.Distance - car.Distance;
                if(delta >= 0.0f && delta < neighborDelta)
                {
                    neighbor = other;
                    neighborDelta = delta;
                }
            }


            //if they are within range
            if(neighborDelta < (config.FollowClearance + (car.Length + neighbor.Length) / 2))
            {
                //if they are slower than us, decelerate until we match their speed
                //if
                // if(leftLaneOK)
                // {
                //     //try to change lanes
                //     int leftLane = car.LaneNumber + 1;
                // }
                // else if(rightLaneOK)
                // {
                //     //try to change lanes
                //     int rightLane = car.LaneNumber + 1;
                // }
                // else
                {
                    //slow down
                    car.Speed = math.max(0.0f, car.Speed - car.Acceleration);
                }
            }
            else
            {
                //speed up
                car.Speed = math.min(car.MaxSpeed, car.Speed + car.Acceleration);
            }

            float xPos = -3.0f + 2.0f * car.LaneNumber;

            //car.Position = new float3(xPos, 0.0f, car.Distance);
            car.Distance += car.Speed * SystemAPI.Time.DeltaTime;

            // TODO: QGU - Fix car transformation
            //float4x4 carTransform = GetWorldTransformation(car.Distance, lane.LaneLength, lane.LaneRadius, 60.0f);
            //state.EntityManager.SetComponentData(car.Self, LocalTransform.FromMatrix(carTransform));

            car.Position = new float3(xPos, 0.0f, car.Distance);
        }
    }

    private static float4x4 GetWorldTransformation(float distance, float laneLength, float laneRadius, float straightLength)
    {
        // Combined segment is a segment with 1 straight and one curve

        float combinedSegmentLength = laneLength * 0.25f;

        int combinedSegmentIndex = (int)math.floor(distance / combinedSegmentLength);
        float distanceInCombinedSegment = distance - combinedSegmentLength * combinedSegmentIndex;

        bool isOnStraight = (distanceInCombinedSegment <= straightLength);

        var rotation = quaternion.RotateY(math.PI * 0.5f * combinedSegmentIndex);

        if (isOnStraight)
        {
            var translation = new float3(-straightLength * 0.5f - laneRadius, 0.0f, distanceInCombinedSegment - straightLength * 0.5f);
            translation = math.rotate(rotation, translation);
            return float4x4.TRS(translation, rotation, new float3(1f, 1f, 1f));
        }
        else
        {
            float distOnCurve = distanceInCombinedSegment - straightLength;
            float angle = distOnCurve * 2.0f * math.PI / laneRadius;
            var translation = new float3(-straightLength * 0.5f - laneRadius * math.cos(angle), 0.0f,
                straightLength * 0.5f + laneRadius * math.sin(angle));

            //var translation = new float3(-straightLength * 0.5f, 0.0f, straightLength * 0.5f);
            translation = math.rotate(rotation, translation);
            return float4x4.TRS(translation, rotation, new float3(1f, 1f, 1f));
        }
    }

    private int ShouldChangeLanes(CarAspect car, Unity.Collections.NativeArray<Car> allCars)
    {
        //look in the left lane
        //look in the right lane


        Car neighbor = allCars[0];//assume there is a car and that we will have one close to us
        float neighborDelta = 1000.0f;
        foreach (var other in allCars)
        {
            if(other.Index == car.Index) { continue; }
            if(other.LaneNumber != car.LaneNumber) { continue; }

            float delta = other.Distance - car.Distance;
            if(delta >= 0.0f && delta < neighborDelta)
            {
                neighbor = other;
                neighborDelta = delta;
            }
        }

        return -1;
    }
}
