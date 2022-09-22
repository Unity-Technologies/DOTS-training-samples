using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;

[UpdateAfter(typeof(TargetingSystem))]
partial struct GatheringSystem : ISystem
{
    private EntityQuery m_GatheringBeesQuery;
    private EntityQuery m_YellowTeamQuery;
    private EntityQuery m_BlueTeamQuery;
    private EntityQuery m_ResourceQuery;

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<BeeConfig>();
        state.RequireForUpdate<ResourceConfig>();
        m_GatheringBeesQuery = state.GetEntityQuery(typeof(IsHolding), ComponentType.Exclude<IsAttacking>(), ComponentType.Exclude<Decay>());
        m_YellowTeamQuery = state.GetEntityQuery(typeof(YellowTeam), typeof(IsHolding), ComponentType.Exclude<IsAttacking>(), ComponentType.Exclude<Decay>());
        m_BlueTeamQuery = state.GetEntityQuery(typeof(BlueTeam), typeof(IsHolding), ComponentType.Exclude<IsAttacking>(), ComponentType.Exclude<Decay>());
        m_ResourceQuery = state.GetEntityQuery(typeof(Holder));
    }

    public void OnDestroy(ref SystemState state)
    {
    }

    public void OnUpdate(ref SystemState state)
    {
        var beeConfig = SystemAPI.GetSingleton<BeeConfig>();
        var resourceConfig = SystemAPI.GetSingleton<ResourceConfig>();
        var ecb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();

        var collectJob = new ResourceCollectingJob()
        {
            DeltaTime = state.Time.DeltaTime,
            GrabDistanceSquared = beeConfig.grabDistance * beeConfig.grabDistance,
            transformLookup = state.GetComponentLookup<LocalToWorldTransform>(),
            ChaseForce = beeConfig.chaseForce,
            ecb = ecb
        }.ScheduleParallel(m_GatheringBeesQuery, state.Dependency);

        var blueBeesReturnJob = new ResourceReturningJob()
        {
            DeltaTime = state.Time.DeltaTime,
            CarryForce = beeConfig.carryForce,
            Hive = BeeTeam.Blue,
            ecb = ecb
        }.ScheduleParallel(m_BlueTeamQuery, state.Dependency);

        var yellowBeesReturnJob = new ResourceReturningJob()
        {
            DeltaTime = state.Time.DeltaTime,
            CarryForce = beeConfig.carryForce,
            Hive = BeeTeam.Yellow,
            ecb = ecb
        }.ScheduleParallel(m_YellowTeamQuery, state.Dependency);
        
        var resourceHoldingJob = new ResourceHoldingJob()
        {
            DeltaTime = state.Time.DeltaTime,
            HolderSize = beeConfig.minBeeSize,
            CarryStiffness = resourceConfig.carryStiffness,
            TransformLookup = state.GetComponentLookup<LocalToWorldTransform>()
        }.ScheduleParallel(m_ResourceQuery, state.Dependency);
    }
}
