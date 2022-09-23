using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[BurstCompile]
[UpdateAfter(typeof(BeeClampingSystem))]
partial struct BeeRenderingSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<BeeConfig>();
        state.RequireForUpdate<FieldConfig>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
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
            up = math.up()
        }.ScheduleParallel();
    }
}