using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

[BurstCompile]
partial struct BeeRenderingSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<BeeConfig>();
        state.RequireForUpdate<FieldConfig>();
    }

    public void OnDestroy(ref SystemState state)
    {
    }

    public void OnUpdate(ref SystemState state)
    {
        var beeConfig = SystemAPI.GetSingleton<BeeConfig>();
        
        new SmoothPositionUpdateJob()
        {
            rotationAmount = Time.deltaTime * beeConfig.rotationStiffness,
            attackingLookup = state.GetComponentLookup<IsAttacking>()
        }.ScheduleParallel();

        new BeeScalingAndRotationJob()
        {
            speedStretch = beeConfig.speedStretch,
            isDeadLookup = state.GetComponentLookup<Decay>()
        }.ScheduleParallel();
    }
}