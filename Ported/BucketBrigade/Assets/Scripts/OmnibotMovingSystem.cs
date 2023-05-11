using Unity.Burst;
using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

// [UpdateAfter(typeof(OmnibotSpawnerSystem))]
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
        foreach (var (omnibotTransform, omnibot) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<Omnibot>>())
        {
            var omnibotState = omnibot.ValueRW.OmnibotState;
            var omnibotPosition = omnibotTransform.ValueRW.Position;
            var elapsedTime = (float)SystemAPI.Time.ElapsedTime;
            switch (omnibotState)
            {
                case OmnibotState.LookForWater:
                    var nearestWaterDistance = math.INFINITY;
                    float3 omniBotTargetPosition = default;
                    Entity foundEntity = default;
                    foreach (var (transformWater, water, waterEntity) in SystemAPI
                                 .Query<RefRO<LocalTransform>, RefRW<Water>>().WithEntityAccess())
                    {
                        var waterPos = transformWater.ValueRO.Position;
                        waterPos.y = 0; // only travel to ground level
                        var distance = math.distance(waterPos, omnibotPosition);
                        if (distance < nearestWaterDistance)
                        {
                            nearestWaterDistance = distance;
                            omniBotTargetPosition = waterPos;
                            foundEntity = waterEntity;
                        }
                    }

                    if (!float.IsPositiveInfinity(nearestWaterDistance))
                    {
                        omnibot.ValueRW.TargetPos = omniBotTargetPosition;
                        omnibot.ValueRW.OmnibotState = OmnibotState.TravelToWater;
                        omnibot.ValueRW.TargetWaterEntity = foundEntity;
                    }

                    break;
                case OmnibotState.TravelToWater:
                    // TODO How do i get Transform from waterLookup[omnibot.ValueRW.targetEntity]

                    var frameTravelDistanceToWater = omnibot.ValueRO.TravelSpeed * SystemAPI.Time.DeltaTime;
                    var remainingDistToWater =
                        math.distance(omnibotTransform.ValueRW.Position, omnibot.ValueRW.TargetPos);

                    // reached destination
                    if (frameTravelDistanceToWater >= remainingDistToWater)
                    {
                        omnibotTransform.ValueRW.Position = omnibot.ValueRW.TargetPos;
                        omnibot.ValueRW.OmnibotState = OmnibotState.GatherWater;
                    }
                    // continue walking
                    else
                    {
                        var travelVecNormalized =
                            math.normalize(omnibot.ValueRW.TargetPos - omnibotTransform.ValueRW.Position);
                        omnibotTransform.ValueRW.Position += travelVecNormalized * frameTravelDistanceToWater;
                    }

                    break;
                case OmnibotState.GatherWater:
                    var targetWater = waterLookup[omnibot.ValueRW.TargetWaterEntity];
                    var waterGatherStillNeeded = omnibot.ValueRO.MaxWaterCapacity - omnibot.ValueRW.CurrentWaterCarryingVolume;
                    var waterVolumeTransfer = omnibot.ValueRO.WaterGatherSpeed * SystemAPI.Time.DeltaTime;

                    // Gathered enough water
                    if (waterVolumeTransfer > waterGatherStillNeeded)
                    {
                        waterVolumeTransfer = waterGatherStillNeeded;
                        omnibot.ValueRW.OmnibotState = OmnibotState.LookForFire;
                    }

                    targetWater.Volume -= waterVolumeTransfer;
                    omnibot.ValueRW.CurrentWaterCarryingVolume += waterVolumeTransfer;

                    // reassign the new component values back to the entity.
                    waterLookup[omnibot.ValueRW.TargetWaterEntity] = targetWater;

                    break;
                case OmnibotState.LookForFire:
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
                            var distance = math.distance(firePos, omnibotPosition);
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
                        omnibot.ValueRW.TargetPos = fireTargetPosition;
                        omnibot.ValueRW.OmnibotState = OmnibotState.TravelToFire;
                        omnibot.ValueRW.TargetFireEntity = foundFireEntity;
                    }

                    break;
                case OmnibotState.TravelToFire:
                    var frameTravelDistanceToFire = omnibot.ValueRO.TravelSpeed * SystemAPI.Time.DeltaTime;
                    var remainingDistToFire =
                        math.distance(omnibotTransform.ValueRW.Position, omnibot.ValueRW.TargetPos);

                    // reached destination
                    if (frameTravelDistanceToFire >= remainingDistToFire)
                    {
                        omnibotTransform.ValueRW.Position = omnibot.ValueRW.TargetPos;
                        omnibot.ValueRW.OmnibotState = OmnibotState.DouseFire;
                    }
                    // continue walking
                    else
                    {
                        var travelVecNormalized =
                            math.normalize(omnibot.ValueRW.TargetPos - omnibotTransform.ValueRW.Position);
                        omnibotTransform.ValueRW.Position += travelVecNormalized * frameTravelDistanceToFire;
                    }

                    break;
                case OmnibotState.DouseFire:
                    // Dump Water
                    omnibot.ValueRW.CurrentWaterCarryingVolume = 0;

                    // Douse Fire
                    foreach (var (transformFire, fire) in SystemAPI
                                 .Query<RefRO<LocalTransform>, RefRW<Fire>>())
                    {
                        if (fire.ValueRO.t > math.EPSILON)
                        {
                            var firePos = transformFire.ValueRO.Position;
                            firePos.y = 0;

                            var distance = math.distance(firePos, omnibotPosition);
                            Debug.Log($"distance = {distance}   :  {omnibot.ValueRO.DouseRadius}");
                            if (distance < omnibot.ValueRO.DouseRadius)
                            {
                                Debug.Log($"Dousing {fire}");
                                var douseAmount = omnibot.ValueRO.MaxDouseAmount *
                                                  (1 - distance / omnibot.ValueRO.DouseRadius);
                                fire.ValueRW.t -= douseAmount;
                                if (fire.ValueRO.t < 0)
                                {
                                    Debug.Log("Max Douse Reached");
                                    fire.ValueRW.t = 0;
                                }
                            }
                        }
                    }


                    
                    omnibot.ValueRW.OmnibotState = OmnibotState.LookForWater;

                    break;
            }
        }
    }
}

[WithAll(typeof(Water))]
[BurstCompile]
public partial struct OmnibotMovingJob : IJobEntity
{
    [ReadOnly] public Water waterEntity;
    public float rate;
    public float spreadVal;

    void Execute([EntityIndexInQuery] int index, ref Fire fire, ref Water water)
    {
    }
}