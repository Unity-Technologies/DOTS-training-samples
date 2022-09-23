using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;

[BurstCompile]
[UpdateAfter(typeof(TargetingSystem))]
partial struct GatheringSystem : ISystem
{
    private EntityQuery m_GatheringBeesQuery;
    private EntityQuery m_YellowTeamQuery;
    private EntityQuery m_BlueTeamQuery;

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<BeeConfig>();
        state.RequireForUpdate<FieldConfig>();
        m_GatheringBeesQuery = state.GetEntityQuery(typeof(IsHolding), ComponentType.Exclude<IsAttacking>(), ComponentType.Exclude<Decay>());
        m_YellowTeamQuery = state.GetEntityQuery(typeof(YellowTeam), typeof(IsHolding), ComponentType.Exclude<IsAttacking>(), ComponentType.Exclude<Decay>());
        m_BlueTeamQuery = state.GetEntityQuery(typeof(BlueTeam), typeof(IsHolding), ComponentType.Exclude<IsAttacking>(), ComponentType.Exclude<Decay>());
    }

    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var beeConfig = SystemAPI.GetSingleton<BeeConfig>();
        var fieldConfig = SystemAPI.GetSingleton<FieldConfig>();
        var ecbs = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var collectJobECB = ecbs.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();
        var blueReturnJobECB = ecbs.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();
        var yellowReturnJobECB = ecbs.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();

        new ResourceCollectingJob()
        {
            DeltaTime = state.Time.DeltaTime,
            GrabDistanceSquared = beeConfig.grabDistance * beeConfig.grabDistance,
            transformLookup = state.GetComponentLookup<LocalToWorldTransform>(),
            ChaseForce = beeConfig.chaseForce,
            ecb = collectJobECB
        }.ScheduleParallel(m_GatheringBeesQuery);

        new ResourceReturningJob()
        {
            DeltaTime = state.Time.DeltaTime,
            CarryForce = beeConfig.carryForce,
            Hive = BeeTeam.Blue,
            ecb = blueReturnJobECB,
            FieldSize = fieldConfig.FieldScale,
            holderLookup = state.GetComponentLookup<Holder>()
        }.ScheduleParallel(m_BlueTeamQuery);

        new ResourceReturningJob()
        {
            DeltaTime = state.Time.DeltaTime,
            CarryForce = beeConfig.carryForce,
            Hive = BeeTeam.Yellow,
            FieldSize = fieldConfig.FieldScale,
            ecb = yellowReturnJobECB,
            holderLookup = state.GetComponentLookup<Holder>()
        }.ScheduleParallel(m_YellowTeamQuery);
    }
}
