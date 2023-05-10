using System;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Entities;
using Unity.Collections;
using Unity.Rendering;
using Unity.Mathematics;
using Unity.Transforms;

public partial struct OmnibotMovingSystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<Water>();
        state.RequireForUpdate<Fire>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        var config = SystemAPI.GetSingleton<Grid>();
        var waterLookup = SystemAPI.GetComponentLookup<Water>(); 
        foreach (var ( transform, omnibot) in SystemAPI.Query< RefRW<LocalTransform>, RefRW<Omnibot>>())
        {
            var omnibotState = omnibot.ValueRW.omnibotState;
            var omnibotPosition = transform.ValueRW.Position;
            float elapsedTime = (float)SystemAPI.Time.ElapsedTime;
            switch (omnibotState)
            {
                case OmnibotState.LookForWater:
                    float nearestWaterDistance = math.INFINITY;
                    float3 omniBotTargetPosition = default;
                    Entity foundEntity = default;
                    foreach (var(transformWater, water, waterEntity) in SystemAPI.Query< RefRO<LocalTransform>, RefRW<Water>>().WithEntityAccess())
                    {
                        var waterPos = transformWater.ValueRO.Position;
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
                        omnibot.ValueRW.OmniboatTargetPos = math.lerp(omnibot.ValueRW.OmniboatTargetPos,omniBotTargetPosition, elapsedTime);
                        omnibot.ValueRW.omnibotState = OmnibotState.TravelToWater;
                        omnibot.ValueRW.targetEntity = foundEntity;
                    }
                    break;
                case OmnibotState.TravelToWater:
                    transform.ValueRW.Position = omnibot.ValueRW.OmniboatTargetPos;
                    omnibot.ValueRW.omnibotState = OmnibotState.GatherWater;
                    break;
                case OmnibotState.GatherWater:
                    var targetWater = waterLookup[omnibot.ValueRW.targetEntity];
                    targetWater.Volume -= .1f;
                    waterLookup[omnibot.ValueRW.targetEntity] = targetWater;
                    omnibot.ValueRW.omnibotState = OmnibotState.LookForFire;
                    break;
                case OmnibotState.LookForFire:
                    float nearestFireDistance = math.INFINITY;
                    float3 fireTargetPosition = default;
                    Entity foundFireEntity = default;
                    foreach (var(transformFire, fire, fireEntity) in SystemAPI.Query< RefRO<LocalTransform>, RefRW<Fire>>().WithEntityAccess())
                    {
                        var firePos = transformFire.ValueRO.Position;
                        var distance = math.distance(firePos, omnibotPosition);
                        if (distance < nearestFireDistance)
                        {
                            nearestFireDistance = distance;
                            fireTargetPosition = firePos;
                            foundFireEntity = fireEntity;
                        }
                    }
                    if (!float.IsPositiveInfinity(nearestFireDistance))
                    {
                        omnibot.ValueRW.OmniboatTargetPos = math.lerp(omnibot.ValueRW.OmniboatTargetPos,fireTargetPosition, elapsedTime);
                        omnibot.ValueRW.omnibotState = OmnibotState.TravelToFire;
                        omnibot.ValueRW.targetEntity = foundFireEntity;
                    }
                    break;
                case OmnibotState.TravelToFire:
                    transform.ValueRW.Position = omnibot.ValueRW.OmniboatTargetPos;
                    omnibot.ValueRW.omnibotState = OmnibotState.TurnOffFire;
                    break;
                case OmnibotState.TurnOffFire:
                    break;
            }
        }
    }
}

[WithAll(typeof(Water))]
[BurstCompile]
public partial struct OmnibotMovingJob : IJobEntity {
    [ReadOnly] public Water waterEntity;
    public float rate;
    public float spreadVal;

    void Execute([EntityIndexInQuery] int index, ref Fire fire, ref Water water) {


    }
}