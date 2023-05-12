using System;
using Miscellaneous.StateChangeEnableable;
using Unity.Burst;
using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(FireSystem))]
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
        var watersQuery = SystemAPI.QueryBuilder().WithAll<Water, LocalTransform>().Build();
        var waters = watersQuery.ToComponentDataArray<Water>(state.WorldUpdateAllocator);
        var waterEntities = watersQuery.ToEntityArray(state.WorldUpdateAllocator);
        var waterTransform = watersQuery.ToComponentDataArray<LocalTransform>(state.WorldUpdateAllocator);

        var fireQuery = SystemAPI.QueryBuilder().WithAll<Fire, LocalTransform>().Build();
        var fires = fireQuery.ToComponentDataArray<Fire>(state.WorldUpdateAllocator);
        var fireEntities = fireQuery.ToEntityArray(state.WorldUpdateAllocator);
        var fireTransform = fireQuery.ToComponentDataArray<LocalTransform>(state.WorldUpdateAllocator);

        var waterLookup = SystemAPI.GetComponentLookup<Water>();
        var fireLookup = SystemAPI.GetComponentLookup<Fire>();
        var teambotLookup = SystemAPI.GetComponentLookup<Teambot>();
        var transformLookup = SystemAPI.GetComponentLookup<LocalTransform>();

        var config = SystemAPI.GetSingleton<Grid>();
        var deltaTime = SystemAPI.Time.DeltaTime;

        var job = new TeambotMovingJob
        {
            waters = waters,
            waterEntities = waterEntities,
            waterTransforms = waterTransform,

            fires = fires,
            fireEntities = fireEntities,
            fireTransforms = fireTransform,
            transformLookup = transformLookup,

            waterLookup = waterLookup,
            fireLookup = fireLookup,
            teambotLookup = teambotLookup,

            config = config,
            deltaTime = deltaTime,
        };
        state.Dependency = job.Schedule(state.Dependency);


        // var config = SystemAPI.GetSingleton<Grid>();
        //
        // var teambotLookup = SystemAPI.GetComponentLookup<Teambot>();
        // var transformLookup = SystemAPI.GetComponentLookup<LocalTransform>();
        // var waterLookup = SystemAPI.GetComponentLookup<Water>();
        // foreach (var (teambotTransform, teambot) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<Teambot>>())
        // {
        //     
        // }
    }
}

[WithAll(typeof(Teambot))]
[BurstCompile] 
public partial struct TeambotMovingJob : IJobEntity
{
    // Water
    [ReadOnly] public NativeArray<Water> waters;
    [ReadOnly] public NativeArray<Entity> waterEntities;
    [ReadOnly] public NativeArray<LocalTransform> waterTransforms;

    // Fire
    [ReadOnly] public NativeArray<Fire> fires;
    [ReadOnly] public NativeArray<Entity> fireEntities;
    [ReadOnly] public NativeArray<LocalTransform> fireTransforms;

    // Lookups
    public ComponentLookup<Water> waterLookup;
    public ComponentLookup<Fire> fireLookup;
    public ComponentLookup<Teambot> teambotLookup;
    public ComponentLookup<LocalTransform> transformLookup;

    // config
    public Grid config;
    public float deltaTime;

    void Execute(Entity teambotEnity)
    {
        var teambot = teambotLookup[teambotEnity];
        var teambotTransform = transformLookup[teambotEnity];

        var teambotRole = teambot.Role;
        var teambotState = teambot.State;
        var teambotPosition = teambotTransform.Position;
        // var elapsedTime = (float)SystemAPI.Time.ElapsedTime;
        switch (teambotRole)
        {
            case TeamBotRole.WaterGatherer: 

                switch (teambotState)
                {
                    case TeamBotState.Init:
                        var nearestWaterDistance = math.INFINITY;
                        Entity foundEntity = default;
                        for (var i = 0; i < waters.Length; i++)
                        {
                            var waterPos = waterTransforms[i].Position;
                            waterPos.y = 0; // only travel to ground level
                            var distance = math.distance(waterPos, teambotPosition);
                            if (distance < nearestWaterDistance)
                            {
                                nearestWaterDistance = distance;
                                foundEntity = waterEntities[i];
                            }
                        }

                        if (!float.IsPositiveInfinity(nearestWaterDistance))
                        {
                            teambot.TargetWaterEntity = foundEntity;
                            teambot.TargetPosition = transformLookup[foundEntity].Position;
                            teambot.TargetPosition.y = 0;
                            teambot.State = TeamBotState.WaterHolder;
                        }

                        break;

                    case TeamBotState.Idle:
                        teambot.waterFillElapsedTime = 0;
                        WalkToPosition(ref teambot, ref teambotTransform, deltaTime,
                            teambot.TargetPosition, config.TeambotTravelSpeed);

                        break;

                    case TeamBotState.WaterHolder:
                        if (teambot.waterFillElapsedTime < config.TeambotWaterFillDuration)
                        {
                            teambot.waterFillElapsedTime += deltaTime;
                            var water = waterLookup[teambot.TargetWaterEntity];
                            water.Volume -= config.TeambotWaterGatherSpeed * deltaTime;
                            waterLookup[teambot.TargetWaterEntity] = water;
                        }
                        else
                        {
                            PassWaterToNextTeamate(ref teambot, ref teambotTransform, transformLookup, teambotLookup,
                                deltaTime, config.TeambotTravelSpeed);
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
                        teambot.State = TeamBotState.Idle;
                        break;
                    case TeamBotState.Idle:
                        var linePos = GetPasserPosition(transformLookup, teambotLookup, ref teambot, out var offsetVec,
                            out var offsetMagnitude, false);

                        teambot.TargetPosition = linePos + (offsetVec * offsetMagnitude);
                        WalkToPosition(ref teambot, ref teambotTransform, deltaTime,
                            teambot.TargetPosition, config.TeambotTravelSpeed);
                        break;
                    case TeamBotState.WaterHolder:
                        PassWaterToNextTeamate(ref teambot, ref teambotTransform, transformLookup, teambotLookup,
                            deltaTime, config.TeambotTravelSpeed);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                break;
            case TeamBotRole.FireDouser:
                switch (teambotState)
                {
                    case TeamBotState.Init:
                        teambot.State = TeamBotState.Idle;
                        break;
                    case TeamBotState.Idle:
                        var waterGatherer = teambotLookup[teambot.TeamWaterGatherer];
                        var targetWaterPos = transformLookup[waterGatherer.TargetWaterEntity].Position;

                        // find nearest fire
                        float nearestFireDistance = math.INFINITY;
                        Entity foundFireEntity = default;
                        for (var i = 0; i < fires.Length; i++)
                        {
                            if (fires[i].t > math.EPSILON)
                            {
                                var firePos = fireTransforms[i].Position;
                                firePos.y = 0; // only travel to ground level
                                var distance = math.distance(firePos, targetWaterPos);
                                if (distance < nearestFireDistance)
                                {
                                    nearestFireDistance = distance;
                                    foundFireEntity = fireEntities[i];
                                }
                            }
                        }

                        if (!float.IsPositiveInfinity(nearestFireDistance))
                        {
                            teambot.TargetFireEntity = foundFireEntity;
                            teambot.TargetPosition = transformLookup[foundFireEntity].Position;
                            teambot.TargetPosition.y = 0;
                        }

                        // Go to fire
                        WalkToPosition(ref teambot, ref teambotTransform, deltaTime,
                            teambot.TargetPosition, config.TeambotTravelSpeed);

                        break;
                    case TeamBotState.WaterHolder:

                        for (var i = 0; i < fires.Length; i++)
                        {
                            var fire = fires[i];
                            if (fire.t > math.EPSILON)
                            {
                                var firePos = fireTransforms[i].Position;
                                firePos.y = 0;

                                var distance = math.distance(firePos, teambotPosition);
                                if (distance < config.TeambotDouseRadius)
                                {
                                    var douseAmount = config.TeambotMaxDouseAmount *
                                                      (1 - distance / config.TeambotDouseRadius);
                                    fire.t -= douseAmount;
                                    if (fire.t < 0)
                                    {
                                        fire.t = 0;
                                    }

                                    fireLookup[fireEntities[i]] = fire;
                                }
                            }
                        }

                        PassWaterToNextTeamate(ref teambot, ref teambotTransform, transformLookup, teambotLookup,
                            deltaTime, config.TeambotTravelSpeed);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }


                break;
            case TeamBotRole.PassTowardsWater:
                switch (teambotState)
                {
                    case TeamBotState.Init:
                        teambot.State = TeamBotState.Idle;
                        break;
                    case TeamBotState.Idle:
                        var linePos = GetPasserPosition(transformLookup, teambotLookup,ref  teambot, out var offsetVec,
                            out var offsetMagnitude, true);
                        teambot.TargetPosition = linePos + (offsetVec * offsetMagnitude);
                        WalkToPosition(ref teambot, ref teambotTransform, deltaTime,
                            teambot.TargetPosition, config.TeambotTravelSpeed);
                        break;
                    case TeamBotState.WaterHolder:
                        PassWaterToNextTeamate(ref teambot, ref teambotTransform, transformLookup, teambotLookup,
                            deltaTime, config.TeambotTravelSpeed);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                break;
            default:
                throw new ArgumentOutOfRangeException();
        }


        teambotLookup[teambotEnity] = teambot;
        transformLookup[teambotEnity] = teambotTransform;
    }

    static void PassWaterToNextTeamate(ref Teambot teambot, ref LocalTransform teambotTransform,
        ComponentLookup<LocalTransform> transformLookup, ComponentLookup<Teambot> teambotLookup, float deltaTime,
        float travelSpeed)
    {
        if (WalkToPosition(ref teambot, ref teambotTransform, deltaTime, transformLookup[teambot.PassToTarget].Position,
                travelSpeed))
        {
            // set next person to be the water holder
            var newWaterHolder = teambotLookup[teambot.PassToTarget];
            newWaterHolder.State = TeamBotState.WaterHolder;
            teambotLookup[teambot.PassToTarget] = newWaterHolder;

            // I'm no longer water holder
            teambot.State = TeamBotState.Idle;
        }
    }

    static float3 GetPasserPosition(ComponentLookup<LocalTransform> transformLookup,
        ComponentLookup<Teambot> teambotLookup, ref Teambot teambot, out float3 offsetVec,
        out float offsetMagnitude, bool flip)
    {
        Teambot waterGatherer = default;
        Teambot fireDouser = default;
        float3 targetWaterPos = default;
        float3 targetFirePos = default;


        if (teambot.TeamWaterGatherer == Entity.Null ||
            teambot.TeamFireDouser == Entity.Null)
        {
            offsetVec = float3.zero;
            offsetMagnitude = 0;
            return float3.zero;
        }

        waterGatherer = teambotLookup[teambot.TeamWaterGatherer];
        fireDouser = teambotLookup[teambot.TeamFireDouser];

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
            teambot.PositionInLine);

        var flipVal = flip ? -1 : 1;

        quaternion rotation = quaternion.RotateY(math.radians(90 * flipVal));
        float3 originalVector =
            math.normalize(targetFirePos - targetWaterPos);
        offsetVec = math.mul(rotation, originalVector);

        float value = teambot.PositionInLine;
        offsetMagnitude = 4 * value * (1 - value) * 2;

        return linePos;
    }

    static bool WalkToPosition(ref Teambot teambot, ref LocalTransform teambotTransform, float deltaTime,
        float3 TargetPos, float travelSpeed)
    {
        var frameTravelDistance = travelSpeed * deltaTime;
        var remainingDistToWater =
            math.distance(teambotTransform.Position, TargetPos);

        // reached destination
        if (frameTravelDistance >= remainingDistToWater)
        {
            teambotTransform.Position = TargetPos;
            return true;
        }
        // continue walking
        else
        {
            var travelVecNormalized =
                math.normalize(TargetPos - teambotTransform.Position);
            teambotTransform.Position += travelVecNormalized * frameTravelDistance;
            return false;
        }
    }
}