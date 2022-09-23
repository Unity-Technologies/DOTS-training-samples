using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;

[BurstCompile]
[UpdateAfter(typeof(TargetingSystem))]
partial struct ResourceMovementSystem : ISystem
{
    private EntityQuery m_ResourceQuery;

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<BeeConfig>();
        state.RequireForUpdate<ResourceConfig>();
        m_ResourceQuery = state.GetEntityQuery(typeof(Holder), ComponentType.Exclude<Falling>(), ComponentType.Exclude<Decay>());
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var beeConfig = SystemAPI.GetSingleton<BeeConfig>();
        var resourceConfig = SystemAPI.GetSingleton<ResourceConfig>();
        var ecb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();
        
        new ResourceHoldingJob()
        {
            DeltaTime = state.Time.DeltaTime,
            HolderSize = beeConfig.minBeeSize,
            CarryStiffness = resourceConfig.carryStiffness,
            TransformLookup = state.GetComponentLookup<LocalToWorldTransform>(),
            ecb = ecb
        }.ScheduleParallel();
    }
}
