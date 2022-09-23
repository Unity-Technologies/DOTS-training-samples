using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

[BurstCompile]
[UpdateAfter(typeof(TargetingSystem))]
partial struct AttackingSystem : ISystem {
    
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<BeeConfig>();
    }

    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        var beeConfig = SystemAPI.GetSingleton<BeeConfig>();
        var ecb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();
        state.Dependency = new AttackingJob() {
            deltaTime = state.Time.DeltaTime,
            chaseForce = beeConfig.chaseForce,
            attackForce = beeConfig.attackForce,
            attackDistanceSquared = beeConfig.attackDistance * beeConfig.attackDistance,
            hitDistanceSquared = beeConfig.hitDistance * beeConfig.hitDistance,
            transformLookup = state.GetComponentLookup<LocalToWorldTransform>(),
            ecb = ecb
        }.ScheduleParallel(state.Dependency);
    }
}
