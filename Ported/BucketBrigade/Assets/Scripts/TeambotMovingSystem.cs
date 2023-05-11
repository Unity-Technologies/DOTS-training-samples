using System;
using Miscellaneous.StateChangeEnableable;
using Unity.Burst;
using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(Grid))]
public partial struct TeambotMovingSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Water>();
        state.RequireForUpdate<Fire>();
        state.RequireForUpdate<Grid>();
    }
 
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<Grid>();

        var teambotLookup = SystemAPI.GetComponentLookup<Teambot>();
        var transformLookup = SystemAPI.GetComponentLookup<LocalTransform>();
        var waterLookup = SystemAPI.GetComponentLookup<Water>();
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
                            var nearestWaterDistance = math.INFINITY;
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
                                    foundEntity = waterEntity;
                                }
                            }

                            if (!float.IsPositiveInfinity(nearestWaterDistance))
                            {
                                teambot.ValueRW.TargetWaterEntity = foundEntity;
                                teambot.ValueRW.TargetPosition = transformLookup[foundEntity].Position;
                                teambot.ValueRW.TargetPosition.y = 0;
                                teambot.ValueRW.State = TeamBotState.WaterHolder;
                            }

                            break;

                        case TeamBotState.Idle:
                            teambot.ValueRW.waterFillElapsedTime = 0;
                            WalkToPosition(teambot, teambotTransform, SystemAPI.Time.DeltaTime,
                                teambot.ValueRW.TargetPosition, config.TeambotTravelSpeed);

                            break;

                        case TeamBotState.WaterHolder:
                            if (teambot.ValueRO.waterFillElapsedTime < config.TeambotWaterFillDuration)
                            {
                                var deltaTime = SystemAPI.Time.DeltaTime;
                                teambot.ValueRW.waterFillElapsedTime += deltaTime;
                                var water = waterLookup[teambot.ValueRO.TargetWaterEntity];
                                water.Volume -= config.TeambotWaterGatherSpeed * deltaTime;
                                waterLookup[teambot.ValueRO.TargetWaterEntity] = water;
                            }
                            else
                            {
                                PassWaterToNextTeamate(teambot, teambotTransform, transformLookup, teambotLookup,
                                    SystemAPI.Time.DeltaTime, config.TeambotTravelSpeed);
                            }

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
                            var linePos = GetPasserPosition(transformLookup, teambotLookup, teambot, out var offsetVec,
                                out var offsetMagnitude, false);

                            teambot.ValueRW.TargetPosition = linePos + (offsetVec * offsetMagnitude);
                            WalkToPosition(teambot, teambotTransform, SystemAPI.Time.DeltaTime,
                                teambot.ValueRW.TargetPosition, config.TeambotTravelSpeed);
                            break;
                        case TeamBotState.WaterHolder:
                            PassWaterToNextTeamate(teambot, teambotTransform, transformLookup, teambotLookup,
                                SystemAPI.Time.DeltaTime, config.TeambotTravelSpeed);
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
                            var waterGatherer = teambotLookup[teambot.ValueRO.TeamWaterGatherer];
                            var targetWaterPos = transformLookup[waterGatherer.TargetWaterEntity].Position;

                            // find nearest fire
                            float nearestFireDistance = math.INFINITY;
                            Entity foundFireEntity = default;
                            foreach (var (transformFire, fire, fireEntity) in SystemAPI
                                         .Query<RefRO<LocalTransform>, RefRW<Fire>>().WithEntityAccess())
                            {
                                if (fire.ValueRO.t > math.EPSILON)
                                {
                                    var firePos = transformFire.ValueRO.Position;
                                    firePos.y = 0; // only travel to ground level
                                    var distance = math.distance(firePos, targetWaterPos);
                                    if (distance < nearestFireDistance)
                                    {
                                        nearestFireDistance = distance;
                                        foundFireEntity = fireEntity;
                                    }
                                }
                            }

                            if (!float.IsPositiveInfinity(nearestFireDistance))
                            {
                                teambot.ValueRW.TargetFireEntity = foundFireEntity;
                                teambot.ValueRW.TargetPosition = transformLookup[foundFireEntity].Position;
                                teambot.ValueRW.TargetPosition.y = 0;
                            }

                            // Go to fire
                            WalkToPosition(teambot, teambotTransform, SystemAPI.Time.DeltaTime,
                                teambot.ValueRW.TargetPosition, config.TeambotTravelSpeed);

                            break;
                        case TeamBotState.WaterHolder:

                            foreach (var (transformFire, fire) in SystemAPI
                                         .Query<RefRO<LocalTransform>, RefRW<Fire>>())
                            {
                                if (fire.ValueRO.t > math.EPSILON)
                                {
                                    var firePos = transformFire.ValueRO.Position;
                                    firePos.y = 0;

                                    var distance = math.distance(firePos, teambotPosition);
                                    if (distance < config.TeambotDouseRadius)
                                    {
                                        var douseAmount = config.TeambotMaxDouseAmount *
                                                          (1 - distance / config.TeambotDouseRadius);
                                        fire.ValueRW.t -= douseAmount;
                                        if (fire.ValueRO.t < 0)
                                        {
                                            fire.ValueRW.t = 0;
                                        }
                                    }
                                }
                            }

                            PassWaterToNextTeamate(teambot, teambotTransform, transformLookup, teambotLookup,
                                SystemAPI.Time.DeltaTime, config.TeambotTravelSpeed);
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
                            var linePos = GetPasserPosition(transformLookup, teambotLookup, teambot, out var offsetVec,
                                out var offsetMagnitude, true);
                            teambot.ValueRW.TargetPosition = linePos + (offsetVec * offsetMagnitude);
                            WalkToPosition(teambot, teambotTransform, SystemAPI.Time.DeltaTime,
                                teambot.ValueRW.TargetPosition, config.TeambotTravelSpeed);
                            break;
                        case TeamBotState.WaterHolder:
                            PassWaterToNextTeamate(teambot, teambotTransform, transformLookup, teambotLookup,
                                SystemAPI.Time.DeltaTime, config.TeambotTravelSpeed);
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

    static void PassWaterToNextTeamate(RefRW<Teambot> teambot, RefRW<LocalTransform> teambotTransform,
        ComponentLookup<LocalTransform> transformLookup, ComponentLookup<Teambot> teambotLookup, float deltaTime,
        float travelSpeed)
    {
        if (WalkToPosition(teambot, teambotTransform, deltaTime, transformLookup[teambot.ValueRO.PassToTarget].Position,
                travelSpeed))
        {
            // set next person to be the water holder
            var newWaterHolder = teambotLookup[teambot.ValueRO.PassToTarget];
            newWaterHolder.State = TeamBotState.WaterHolder;
            teambotLookup[teambot.ValueRO.PassToTarget] = newWaterHolder;

            // I'm no longer water holder
            teambot.ValueRW.State = TeamBotState.Idle;
        }
    }

    static float3 GetPasserPosition(ComponentLookup<LocalTransform> transformLookup,
        ComponentLookup<Teambot> teambotLookup, RefRW<Teambot> teambot, out float3 offsetVec,
        out float offsetMagnitude, bool flip)
    {
        Teambot waterGatherer = default;
        Teambot fireDouser = default;
        float3 targetWaterPos = default;
        float3 targetFirePos = default;


        if (teambot.ValueRO.TeamWaterGatherer == Entity.Null ||
            teambot.ValueRO.TeamFireDouser == Entity.Null)
        {
            offsetVec = float3.zero;
            offsetMagnitude = 0;
            return float3.zero;
        }
        
        waterGatherer = teambotLookup[teambot.ValueRO.TeamWaterGatherer];
        fireDouser = teambotLookup[teambot.ValueRO.TeamFireDouser];

        if (waterGatherer.TargetWaterEntity == Entity.Null ||
            fireDouser.TargetFireEntity == Entity.Null)
        {
            offsetVec = float3.zero;
            offsetMagnitude = 0;
            return float3.zero;
        }

        targetWaterPos = transformLookup[waterGatherer.TargetWaterEntity].Position;
        targetWaterPos.y = 0;
        targetFirePos = transformLookup[fireDouser.TargetFireEntity].Position;
        targetFirePos.y = 0;


        var linePos = math.lerp(
            targetWaterPos,
            targetFirePos,
            teambot.ValueRO.PositionInLine);

        var flipVal = flip ? -1 : 1;

        quaternion rotation = quaternion.RotateY(math.radians(90 * flipVal));
        float3 originalVector =
            math.normalize(targetFirePos - targetWaterPos);
        offsetVec = math.mul(rotation, originalVector);

        float value = teambot.ValueRO.PositionInLine;
        offsetMagnitude = 4 * value * (1 - value) * 2;

        return linePos;
    }

    static bool WalkToPosition(RefRW<Teambot> teambot, RefRW<LocalTransform> teambotTransform, float deltaTime,
        float3 TargetPos, float travelSpeed)
    {
        var frameTravelDistance = travelSpeed * deltaTime;
        var remainingDistToWater =
            math.distance(teambotTransform.ValueRW.Position, TargetPos);

        // reached destination
        if (frameTravelDistance >= remainingDistToWater)
        {
            teambotTransform.ValueRW.Position = TargetPos;
            return true;
        }
        // continue walking
        else
        {
            var travelVecNormalized =
                math.normalize(TargetPos - teambotTransform.ValueRW.Position);
            teambotTransform.ValueRW.Position += travelVecNormalized * frameTravelDistance;
            return false;
        }
    }
}