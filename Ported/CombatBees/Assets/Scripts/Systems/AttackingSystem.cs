using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

[BurstCompile]
[UpdateAfter(typeof(TargetingSystem))]
partial struct AttackingSystem : ISystem {
    
    private ComponentLookup<LocalToWorldTransform> transformLookup;
    
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<BeeConfig>();
        transformLookup = state.GetComponentLookup<LocalToWorldTransform>();
    }

    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        var beeConfig = SystemAPI.GetSingleton<BeeConfig>();
        var ecb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();
        transformLookup.Update(ref state);
        state.Dependency = new AttackingJob() {
            deltaTime = state.Time.DeltaTime,
            chaseForce = beeConfig.chaseForce,
            attackForce = beeConfig.attackForce,
            attackDistanceSquared = beeConfig.attackDistance * beeConfig.attackDistance,
            hitDistanceSquared = beeConfig.hitDistance * beeConfig.hitDistance,
            transformLookup = transformLookup,
            ecb = ecb
        }.ScheduleParallel(state.Dependency);
    }
}
