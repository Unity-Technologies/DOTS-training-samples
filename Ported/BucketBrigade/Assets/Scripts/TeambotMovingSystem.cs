using System;
using Unity.Burst;
using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

// [UpdateAfter(typeof(OmnibotSpawnerSystem))]
public partial struct TeambotMovingSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Water>();
        state.RequireForUpdate<Fire>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<Grid>();
        var teambotLookup = SystemAPI.GetComponentLookup<Teambot>();
        // var waterLookup = SystemAPI.GetComponentLookup<Water>();
        // var fireLookup = SystemAPI.GetComponentLookup<Fire>();
        var transformLookup = SystemAPI.GetComponentLookup<LocalTransform>();
        foreach (var (teambotTransform, teambot) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<Teambot>>())
        {
            var teambotRole = teambot.ValueRW.Role;
            var teambotState = teambot.ValueRW.State;
            var teambotPosition = teambotTransform.ValueRW.Position;
            // var elapsedTime = (float)SystemAPI.Time.ElapsedTime;
            switch (teambotRole)
            {
                case TeamBotRole.WaterGatherer:

                    switch (teambotState)
                    {
                        case TeamBotState.Init:
                            Debug.Log("WaterGatherer");
                            var nearestWaterDistance = math.INFINITY;
                            float3 foundTargetPosition = default;
                            Entity foundEntity = default;
                            foreach (var (transformWater, water, waterEntity) in SystemAPI
                                         .Query<RefRO<LocalTransform>, RefRW<Water>>().WithEntityAccess())
                            {
                                var waterPos = transformWater.ValueRO.Position;
                                waterPos.y = 0; // only travel to ground level
                                var distance = math.distance(waterPos, teambotPosition);
                                if (distance < nearestWaterDistance)
                                {
                                    nearestWaterDistance = distance;
                                    foundTargetPosition = waterPos;
                                    foundEntity = waterEntity;
                                }
                            }

                            if (!float.IsPositiveInfinity(nearestWaterDistance))
                            {
                                teambot.ValueRW.TargetPosition = foundTargetPosition;
                                teambot.ValueRW.TargetWaterEntity = foundEntity;
                                teambot.ValueRW.State = TeamBotState.Idle;
                            }

                            break;

                        case TeamBotState.Idle:
                            WalkToTargetPosition(teambot, teambotTransform, SystemAPI.Time.DeltaTime);

                            break;

                        case TeamBotState.Active:
                            break;

                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    break;
                case TeamBotRole.PassTowardsFire:
                    switch (teambotState)
                    {
                        case TeamBotState.Init:
                            teambot.ValueRW.State = TeamBotState.Idle;
                            break;
                        case TeamBotState.Idle:
                            var linePos = GetPasserPosition(transformLookup, teambot, out var offsetVec, out var offsetMagnitude, false);
                            teambot.ValueRW.TargetPosition = linePos + (offsetVec * offsetMagnitude);
                            WalkToTargetPosition(teambot, teambotTransform, SystemAPI.Time.DeltaTime);
                            break;
                        case TeamBotState.Active:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    break;
                case TeamBotRole.FireDouser:
                    switch (teambotState)
                    {
                        case TeamBotState.Init:
                            teambot.ValueRW.State = TeamBotState.Idle;
                            break;
                        case TeamBotState.Idle:
                            var waterGathererPos = transformLookup[teambot.ValueRO.TeamWaterGatherer];

                            // find nearest fire
                            float nearestFireDistance = math.INFINITY;
                            float3 fireTargetPosition = default;
                            Entity foundFireEntity = default;
                            foreach (var (transformFire, fire, fireEntity) in SystemAPI
                                         .Query<RefRO<LocalTransform>, RefRW<Fire>>().WithEntityAccess())
                            {
                                if (fire.ValueRO.t > math.EPSILON)
                                {
                                    var firePos = transformFire.ValueRO.Position;
                                    firePos.y = 0; // only travel to ground level
                                    var distance = math.distance(firePos, waterGathererPos.Position);
                                    if (distance < nearestFireDistance)
                                    {
                                        nearestFireDistance = distance;
                                        fireTargetPosition = firePos;
                                        foundFireEntity = fireEntity;
                                    }
                                }
                            }

                            if (!float.IsPositiveInfinity(nearestFireDistance))
                            {
                                teambot.ValueRW.TargetPosition = fireTargetPosition;
                                teambot.ValueRW.TargetFireEntity = foundFireEntity;
                            }

                            // Go to fire
                            WalkToTargetPosition(teambot, teambotTransform, SystemAPI.Time.DeltaTime);

                            break;
                        case TeamBotState.Active:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }


                    break;
                case TeamBotRole.PassTowardsWater:
                    switch (teambotState)
                    {
                        case TeamBotState.Init:
                            teambot.ValueRW.State = TeamBotState.Idle;
                            break;
                        case TeamBotState.Idle:
                            var linePos = GetPasserPosition(transformLookup, teambot, out var offsetVec, out var offsetMagnitude, true);
                            teambot.ValueRW.TargetPosition = linePos + (offsetVec * offsetMagnitude);
                            WalkToTargetPosition(teambot, teambotTransform, SystemAPI.Time.DeltaTime);
                            break;
                        case TeamBotState.Active:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    static float3 GetPasserPosition(ComponentLookup<LocalTransform> transformLookup, RefRW<Teambot> teambot, out float3 offsetVec,
        out float offsetMagnitude, bool flip)
    {
        var linePos = math.lerp(
            transformLookup[teambot.ValueRO.TeamWaterGatherer].Position,
            transformLookup[teambot.ValueRO.TeamFireDouser].Position,
            teambot.ValueRO.PositionInLine);

        var flipVal = flip ? -1 : 1;

        quaternion rotation = quaternion.RotateY(math.radians(90 * flipVal));
        float3 originalVector =
            math.normalize(transformLookup[teambot.ValueRO.TeamFireDouser].Position -
                           transformLookup[teambot.ValueRO.TeamWaterGatherer].Position);
        offsetVec = math.mul(rotation, originalVector);

        float value = teambot.ValueRO.PositionInLine;
        offsetMagnitude = 4 * value * (1 - value) * 2;
        return linePos;
    }

    static bool WalkToTargetPosition(RefRW<Teambot> teambot, RefRW<LocalTransform> teambotTransform, float deltaTime)
    {
        var frameTravelDistance = teambot.ValueRO.TravelSpeed * deltaTime;
        var remainingDistToWater =
            math.distance(teambotTransform.ValueRW.Position, teambot.ValueRW.TargetPosition);

        // reached destination
        if (frameTravelDistance >= remainingDistToWater)
        {
            teambotTransform.ValueRW.Position = teambot.ValueRW.TargetPosition;
            return true;
        }
        // continue walking
        else
        {
            var travelVecNormalized =
                math.normalize(teambot.ValueRW.TargetPosition - teambotTransform.ValueRW.Position);
            teambotTransform.ValueRW.Position += travelVecNormalized * frameTravelDistance;
            return false;
        }
    }
}