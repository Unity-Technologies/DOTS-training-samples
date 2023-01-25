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
            Car neighbor = allCars[0];//assume there is a car and that we will have one close to us
            float neighborDelta = 1000.0f;

            bool leftLaneOK = car.LaneNumber != config.NumLanes - 1;//set to false if we are at the leftmost lane
            bool rightLaneOK = car.LaneNumber != 0;//set to false if we are at the rightmost lane

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
                if(leftLaneOK)
                {
                    car.LaneNumber++;
                }
                else if(rightLaneOK)
                {
                    car.LaneNumber--;
                }
                else
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

            var lane = lanes[car.LaneNumber];
            car.Distance += car.Speed * SystemAPI.Time.DeltaTime;
            if (car.Distance >= lane.LaneLength)
            {
                car.Distance -= lane.LaneLength;
            }

            float4x4 carTransform = GetWorldTransformation(car.Distance, lane.LaneLength, lane.LaneRadius, Config.SegmentLength * config.TrackSize);
            state.EntityManager.SetComponentData(car.Self, LocalTransform.FromMatrix(carTransform));
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
            float angle = distOnCurve / (laneRadius);

            float4x4 transformation = float4x4.identity;
            transformation = math.mul(float4x4.Translate(new float3(-laneRadius, 0.0f, 0.0f)), transformation);
            transformation = math.mul(float4x4.RotateY(angle), transformation);
            transformation = math.mul(float4x4.Translate(new float3(-straightLength * 0.5f, 0.0f, straightLength * 0.5f)), transformation);
            transformation = math.mul(float4x4.RotateY(math.PI * 0.5f * combinedSegmentIndex), transformation);
            return transformation;
        }
    }
}
