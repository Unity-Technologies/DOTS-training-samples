using Unity.Burst;
using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(OmnibotSpawnerSystem))]
public partial struct OmnibotMovingSystem : ISystem 
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
        var waterLookup = SystemAPI.GetComponentLookup<Water>();
        var fireLookup = SystemAPI.GetComponentLookup<Fire>();
        var watersQuery = SystemAPI.QueryBuilder().WithAll<Water, LocalTransform>().Build();
        var waterEntities = watersQuery.ToEntityArray(state.WorldUpdateAllocator);
        var waterTransform = watersQuery.ToComponentDataArray<LocalTransform>(state.WorldUpdateAllocator);
        var waters = watersQuery.ToComponentDataArray<Water>(state.WorldUpdateAllocator);
        var fireQuery = SystemAPI.QueryBuilder().WithAll<Fire, LocalTransform>().Build();
        var fireEntities = fireQuery.ToEntityArray(state.WorldUpdateAllocator);
        var fireTransform = fireQuery.ToComponentDataArray<LocalTransform>(state.WorldUpdateAllocator);

        var fires = fireQuery.ToComponentDataArray<Fire>(state.WorldUpdateAllocator);

        var elapsedTime = (float)SystemAPI.Time.ElapsedTime;
        var frameTravelDistanceToWater = 5 * SystemAPI.Time.DeltaTime;
        var deltaTime = SystemAPI.Time.DeltaTime;

        var job = new OmnibotMovingJob
        {
            waters = waters,
            waterEntities = waterEntities,
            fires = fires,
            rate = 0,
            spreadVal = 0,
            waterLookup = waterLookup,
            elapsedTime = elapsedTime,
            frameTravelDistanceToWater = frameTravelDistanceToWater,
            deltaTime = deltaTime,
            fireEntities = fireEntities,
            fireLookup = fireLookup,
            waterTransforms = waterTransform,
            fireTransforms = fireTransform
        };
        state.Dependency = job.Schedule(state.Dependency);
    }
}

[WithAll(typeof(Omnibot))]
[BurstCompile]
public partial struct OmnibotMovingJob : IJobEntity
{
    [ReadOnly] public NativeArray<Water> waters;
    [ReadOnly] public NativeArray<Entity> waterEntities;
    [ReadOnly] public NativeArray<LocalTransform> waterTransforms;
    [ReadOnly] public NativeArray<LocalTransform> fireTransforms;

    [ReadOnly] public NativeArray<Fire> fires;
    public float rate;
    public float spreadVal;
    public ComponentLookup<Water> waterLookup;
    public float elapsedTime;
    public float frameTravelDistanceToWater;
    public float deltaTime;
    [ReadOnly] public NativeArray<Entity> fireEntities; 
    public ComponentLookup<Fire> fireLookup;

    void Execute(ref Omnibot omnibot,
        ref LocalTransform omnibotTransform)
    {
        
            switch (omnibot.OmnibotState)
            {
                case OmnibotState.LookForWater:
                    var nearestWaterDistance = math.INFINITY;
                    float3 omniBotTargetPosition = default;
                    Entity foundEntity = default;
                    for (var index = 0; index < waters.Length; index++)
                    {
                        var water = waters[index];
                        var waterPos = waterTransforms[index].Position;
                        waterPos.y = 0; // only travel to ground level
                        var distance = math.distance(waterPos, omnibotTransform.Position);
                        if (distance < nearestWaterDistance)
                        {
                            nearestWaterDistance = distance;
                            omniBotTargetPosition = waterPos;
                            foundEntity = waterEntities[index];
                        }
                    }

                    if (!float.IsPositiveInfinity(nearestWaterDistance))
                    {
                        omnibot.TargetPos = omniBotTargetPosition;
                        omnibot.OmnibotState = OmnibotState.TravelToWater;
                        omnibot.TargetWaterEntity = foundEntity;
                    }

                    break;
                case OmnibotState.TravelToWater:
                    // TODO How do i get Transform from waterLookup[omnibot.ValueRW.targetEntity]

                    
                    var remainingDistToWater =
                        math.distance(omnibotTransform.Position, omnibot.TargetPos);

                    // reached destination
                    if (frameTravelDistanceToWater >= remainingDistToWater)
                    {
                        omnibotTransform.Position = omnibot.TargetPos;
                        omnibot.OmnibotState = OmnibotState.GatherWater;
                    }
                    // continue walking
                    else
                    {
                        var travelVecNormalized =
                            math.normalize(omnibot.TargetPos - omnibotTransform.Position);
                        omnibotTransform.Position += travelVecNormalized * frameTravelDistanceToWater;
                    }

                    break;
                case OmnibotState.GatherWater:
                    var targetWater = waterLookup[omnibot.TargetWaterEntity];
                    var waterGatherStillNeeded = omnibot.MaxWaterCapacity - omnibot.CurrentWaterCarryingVolume;
                    var waterVolumeTransfer = omnibot.WaterGatherSpeed * elapsedTime;

                    // Gathered enough water
                    if (waterVolumeTransfer > waterGatherStillNeeded)
                    {
                        waterVolumeTransfer = waterGatherStillNeeded;
                        omnibot.OmnibotState = OmnibotState.LookForFire;
                    }

                    targetWater.Volume -= waterVolumeTransfer;
                    omnibot.CurrentWaterCarryingVolume += waterVolumeTransfer;

                    // reassign the new component values back to the entity.
                    waterLookup[omnibot.TargetWaterEntity] = targetWater;

                    break;
                case OmnibotState.LookForFire:
                    float nearestFireDistance = math.INFINITY;
                    float3 fireTargetPosition = default;
                    Entity foundFireEntity = default;
                    for (var index = 0; index < fires.Length; index++)
                    {
                        var fire = fires[index];
                        if (fire.t > math.EPSILON)
                        {
                            var firePos = fireTransforms[index].Position;
                            firePos.y = 0; // only travel to ground level
                            var distance = math.distance(firePos, omnibotTransform.Position);
                            if (distance < nearestFireDistance)
                            {
                                nearestFireDistance = distance;
                                fireTargetPosition = firePos;
                                foundFireEntity = fireEntities[index];
                            }
                        }
                    }

                    if (!float.IsPositiveInfinity(nearestFireDistance))
                    {
                        omnibot.TargetPos = fireTargetPosition;
                        omnibot.OmnibotState = OmnibotState.TravelToFire;
                        omnibot.TargetFireEntity = foundFireEntity;
                    }

                    break;
                case OmnibotState.TravelToFire:
                    var frameTravelDistanceToFire = omnibot.TravelSpeed * deltaTime;
                    var remainingDistToFire =
                        math.distance(omnibotTransform.Position, omnibot.TargetPos);

                    // reached destination
                    if (frameTravelDistanceToFire >= remainingDistToFire)
                    {
                        omnibotTransform.Position = omnibot.TargetPos;
                        omnibot.OmnibotState = OmnibotState.DouseFire;
                    }
                    // continue walking
                    else
                    {
                        var travelVecNormalized =
                            math.normalize(omnibot.TargetPos - omnibotTransform.Position);
                        omnibotTransform.Position += travelVecNormalized * frameTravelDistanceToFire;
                    }

                    break;
                case OmnibotState.DouseFire:
                    // Dump Water
                    omnibot.CurrentWaterCarryingVolume = 0;

                    for (var index = 0; index < fires.Length; index++)
                    {
                        var fire = fires[index];
                        if (fire.t > math.EPSILON)
                        {
                            var firePos = fireTransforms[index].Position;
                            firePos.y = 0;
                    
                            var distance = math.distance(firePos, omnibotTransform.Position);
                            if (distance < omnibot.DouseRadius)
                            {
                                var douseAmount = omnibot.MaxDouseAmount *
                                                  (1 - distance / omnibot.DouseRadius);
                                fire.t -= douseAmount;
                                if (fire.t < 0)
                                {
                                    fire.t = 0;
                                }
                    
                                fireLookup[fireEntities[index]] = fire;
                            }
                        }
                    }
                    Debug.LogWarning("HERE");
                    omnibot.OmnibotState = OmnibotState.LookForWater;
                    break;
            }
    }
}