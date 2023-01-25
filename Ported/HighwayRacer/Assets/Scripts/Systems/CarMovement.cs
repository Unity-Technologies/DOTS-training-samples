using Aspects;
using Authoring;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Burst;
using Unity.Collections;
using Unity.Transforms;
using UnityEditor.Search;
using UnityEngine.UIElements;

[BurstCompile]
public partial struct CarMovement : ISystem
{
    private EntityQuery laneQuery;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Config>();
        laneQuery = SystemAPI.QueryBuilder().WithAll<Lane>().Build();
        state.RequireForUpdate(laneQuery);
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

        foreach (var car in SystemAPI.Query<CarAspect>())
        {
            var lane = lanes[car.LaneNumber];

            if (car.Distance > 240.0)
            {
                car.Distance = 0.0f;
            }
            float xPos = -3.0f + 2.0f * car.LaneNumber;
            //car.Position = new float3(xPos, 0.0f, car.Distance);
            car.Distance += car.Speed * SystemAPI.Time.DeltaTime;

            float4x4 carTransform = GetWorldTransformation(car.Distance, lane.LaneLength, lane.LaneRadius, 60.0f);

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
            float angle = distOnCurve * 2.0f * math.PI / laneRadius;
            var translation = new float3(-straightLength * 0.5f - laneRadius * math.cos(angle), 0.0f,
                straightLength * 0.5f + laneRadius * math.sin(angle));

            //var translation = new float3(-straightLength * 0.5f, 0.0f, straightLength * 0.5f);
            translation = math.rotate(rotation, translation);
            return float4x4.TRS(translation, rotation, new float3(1f, 1f, 1f));
        }
    }
}
