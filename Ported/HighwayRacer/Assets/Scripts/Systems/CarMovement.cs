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
                if (other.Index == car.Index) { continue; }

                float min = car.Distance - config.LaneChangeClearance;
                float max = car.Distance + config.LaneChangeClearance;

                int leftLane = car.LaneNumber + 1;
                int rightLane = car.LaneNumber - 1;

                //need to compute the game in terms of the other lane's distance

                if ((other.LaneNumber == leftLane || other.NewLaneNumber == leftLane) && other.Distance > min && other.Distance < max)
                {
                    leftLaneOK = false;
                }

                if ((other.LaneNumber == rightLane || other.NewLaneNumber == rightLane) && other.Distance > min && other.Distance < max)
                {
                    rightLaneOK = false;
                }

                if (other.LaneNumber != car.LaneNumber) { continue; }

                float delta = other.Distance - car.Distance;
                if (delta >= 0.0f && delta < neighborDelta)
                {
                    neighbor = other;
                    neighborDelta = delta;
                }
            }

            // If we're passing, our allowable speed changes from the cars desired speed to the percentage increase allowable while passing
            // var targetSpeed = car.IsPassing ? car.DesiredSpeed * config.MaxSpeedIncreaseWhilePassing : car.DesiredSpeed;
            var targetSpeed = car.DesiredSpeed;

            float laneRadius = 0.0f;
            float laneLength = 0.0f;
            // if (car.LaneChangeProgress >= 0.0f)
            if (car.NewLaneNumber >= 0)
            {
                targetSpeed = neighbor.Speed;
                //just worry about the transition
                car.LaneChangeProgress += SystemAPI.Time.DeltaTime;

                if (car.LaneChangeProgress > config.LaneChangeTime)
                {
                    //finish it up
                    //keep our distance the same, relative to the new lane we are in
                    float percent = car.Distance / lanes[car.LaneNumber].LaneLength;
                    car.Distance = percent * lanes[car.NewLaneNumber].LaneLength;

                    car.LaneNumber = car.NewLaneNumber;
                    car.NewLaneNumber = -1;
                    car.LaneChangeProgress = -1.0f;


                    var lane = lanes[car.LaneNumber];
                    laneRadius = lane.LaneRadius;
                    laneLength = lane.LaneLength;
                    car.IsPassing = false;
                }
                else
                {
                    //do some percentage between
                    float percent = car.LaneChangeProgress / config.LaneChangeTime;
                    var oldLane = lanes[car.LaneNumber];
                    var newLane = lanes[car.NewLaneNumber];
                    laneRadius = math.lerp(oldLane.LaneRadius, newLane.LaneRadius, percent);
                    // laneLength = math.lerp(oldLane.LaneLength, newLane.LaneLength, percent);
                    laneLength = oldLane.LaneLength;//keeps us changing lane parallel, without speeding up due to length change
                }
            }
            else
            {
                // Give priority to merging back into the right lane
                if (rightLaneOK)
                {
                    car.LaneNumber--;
                    car.NewLaneNumber = car.LaneNumber - 1;
                    car.LaneChangeProgress = 0.0f;
                    car.IsPassing = false;
                }
                else if (neighborDelta < (config.FollowClearance + (car.Length + neighbor.Length) / 2))
                {
                    // Otherwise if we're being blocked by a car in our lane, attempt to change left and pass
                    if (leftLaneOK)
                    {
                        // car.LaneNumber++;
                        car.NewLaneNumber = car.LaneNumber + 1;
                        car.LaneChangeProgress = 0.0f;
                        car.IsPassing = true;
                    }
                    // We can't pass, match our target speed to our neighbour
                    targetSpeed = neighbor.Speed;
                }


                var lane = lanes[car.LaneNumber];
                laneRadius = lane.LaneRadius;
                laneLength = lane.LaneLength;
            }

            // If we're currently going less than our target, increase speed based on our acceleration and if we're going faster, decelerate
            // if (car.Speed < targetSpeed)
            // {
            //     car.Speed = math.min(targetSpeed, car.Speed + car.Acceleration);
            // }
            // else
            // {
            //     car.Speed = math.max(targetSpeed, car.Speed - car.Acceleration);
            // }
            car.Speed = targetSpeed;

            car.Distance += car.Speed * SystemAPI.Time.DeltaTime;
            if (car.Distance >= laneLength)
            {
                car.Distance -= laneLength;
            }

            float4x4 carTransform = GetWorldTransformation(car.Distance, laneLength, laneRadius, Config.SegmentLength * config.TrackSize);
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

    // Get distance of two cars on the track
    private static float GetDistance(in float3 currentCarPosition, in float3 otherCarPosition)
    {
        return math.length(currentCarPosition - otherCarPosition);
    }

    private static float GetDistanceOnLaneChange(int currentLane, float currentDistance, int laneChangeTo, in NativeArray<Lane> lanes, float straightLength, float laneOffset)
    {
        if (laneChangeTo < 0 || laneChangeTo >= lanes.Length)
        {
            return -1.0f;
        }

        int laneDiff = laneChangeTo - currentLane;
        ;
        // we should only allow one lane change
        if (math.abs(laneDiff) != 1)
        {
            return -1.0f;
        }

        var currentLaneLength = lanes[currentLane].LaneLength;
        float combinedSegmentLength = currentLaneLength * 0.25f;
        int combinedSegmentIndex = (int)math.floor(currentDistance / combinedSegmentLength);
        float distanceInCombinedSegment = currentDistance - combinedSegmentLength * combinedSegmentIndex;

        float baseOffset = laneDiff * laneOffset * 0.5f * math.PI * combinedSegmentIndex;
        bool isOnStraight = (distanceInCombinedSegment <= straightLength);

        if (isOnStraight)
        {
            return currentDistance + baseOffset;
        }
        else
        {
            float distOnCurve = distanceInCombinedSegment - straightLength;
            float angle = distOnCurve / (lanes[currentLane].LaneRadius);
            return lanes[laneChangeTo].LaneLength * 0.25f * combinedSegmentIndex + straightLength + angle * lanes[laneChangeTo].LaneRadius;

        }
    }
}
