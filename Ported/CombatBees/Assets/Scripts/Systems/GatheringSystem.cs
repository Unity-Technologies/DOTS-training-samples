using Unity.Entities;
using Unity.Transforms;

[UpdateAfter(typeof(TargetingSystem))]
partial struct GatheringSystem : ISystem
{
    private EntityQuery m_GatheringBeesQuery;
    private EntityQuery m_YellowTeamQuery;
    private EntityQuery m_BlueTeamQuery;

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<BeeConfig>();
        m_GatheringBeesQuery = state.GetEntityQuery(typeof(IsHolding), ComponentType.Exclude<IsAttacking>(), ComponentType.Exclude<Decay>());
        m_YellowTeamQuery = state.GetEntityQuery(typeof(YellowTeam), typeof(IsHolding), ComponentType.Exclude<IsAttacking>(), ComponentType.Exclude<Decay>());
        m_BlueTeamQuery = state.GetEntityQuery(typeof(BlueTeam), typeof(IsHolding), ComponentType.Exclude<IsAttacking>(), ComponentType.Exclude<Decay>());
    }

    public void OnDestroy(ref SystemState state)
    {
    }

    public void OnUpdate(ref SystemState state)
    {
        var beeConfig = SystemAPI.GetSingleton<BeeConfig>();
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
            transformLookup = state.GetComponentLookup<LocalToWorldTransform>(),
            Hive = BeeTeam.Blue,
            ecb = ecb
        }.ScheduleParallel(m_BlueTeamQuery, state.Dependency);

        var yellowBeesReturnJob = new ResourceReturningJob()
        {
            DeltaTime = state.Time.DeltaTime,
            CarryForce = beeConfig.carryForce,
            transformLookup = state.GetComponentLookup<LocalToWorldTransform>(),
            Hive = BeeTeam.Yellow,
            ecb = ecb
        }.ScheduleParallel(m_YellowTeamQuery, state.Dependency);
    }
}
