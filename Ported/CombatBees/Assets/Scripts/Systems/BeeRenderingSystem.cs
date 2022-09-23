using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[BurstCompile]
[UpdateAfter(typeof(BeeClampingSystem))]
partial struct BeeRenderingSystem : ISystem
{
    private ComponentLookup<IsAttacking> isAttackingLookup;
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<BeeConfig>();
        state.RequireForUpdate<FieldConfig>();
        isAttackingLookup = state.GetComponentLookup<IsAttacking>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var beeConfig = SystemAPI.GetSingleton<BeeConfig>();
        isAttackingLookup.Update(ref state);
        
        new SmoothPositionUpdateJob()
        {
            rotationAmount = Time.deltaTime * beeConfig.rotationStiffness,
            attackingLookup = isAttackingLookup
        }.ScheduleParallel();

        new BeeScalingAndRotationJob()
        {
            speedStretch = beeConfig.speedStretch,
            up = math.up()
        }.ScheduleParallel();
    }
}