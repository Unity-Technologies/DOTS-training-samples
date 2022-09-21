using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

[BurstCompile]
partial struct TargetingEnemyJob : IJobEntity
{
    public EntityCommandBuffer.ParallelWriter ECB;
    
    public float aggression;

    [ReadOnly] public NativeArray<Entity> enemies;

    void Execute(Entity bee, [EntityInQueryIndex] int idx)
    {
        if (enemies.Length == 0)
        {
            return;
        }

        var random = Random.CreateFromIndex((uint)idx);
        if (random.NextFloat() < aggression)
        {
            ECB.SetComponentEnabled<TargetId>(idx, bee, true);
            ECB.SetComponent(idx, bee, new TargetId() { Value = enemies[random.NextInt(0, enemies.Length)] });
        }
    }
}

[BurstCompile]
[WithAny(typeof(YellowTeam), typeof(BlueTeam))]
[WithNone(typeof(TargetId))]
partial struct TargetingResourceJob : IJobEntity
{
    public EntityCommandBuffer.ParallelWriter ECB;

    [ReadOnly] public NativeArray<Entity> resources;

    void Execute(Entity bee, [EntityInQueryIndex] int idx)
    {
        var random = Random.CreateFromIndex((uint)idx);
        ECB.SetComponentEnabled<TargetId>(idx, bee, true);
        ECB.SetComponent(idx, bee, new TargetId() { Value = resources[random.NextInt(0, resources.Length)] });
    }
}

[WithNone(typeof(IsHolding))]
partial struct TargetingUpdateJob : IJobEntity
{
    public EntityCommandBuffer ECB;

    [ReadOnly] public ComponentLookup<Holder> holders;
    [ReadOnly] public NativeHashSet<Entity> friends;

    void Execute(Entity bee, TargetId target)
    {
        if (holders.TryGetComponent(target.Value, out Holder holder) && !friends.Contains(holder.Value))
        {
            ECB.SetComponent(bee, new TargetId() { Value = holder.Value });
        }
    }
}

//[BurstCompile]
partial struct TargetingSystem : ISystem
{
    private EntityQuery m_yellowTeamQuery;
    private EntityQuery m_yellowTeamIdleQuery;
    private EntityQuery m_blueTeamQuery;
    private EntityQuery m_blueTeamIdleQuery;
    private EntityQuery m_resourcesQuery;

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<BeeConfig>();

        m_yellowTeamQuery = state.GetEntityQuery(typeof(YellowTeam));
        m_yellowTeamIdleQuery = state.GetEntityQuery(typeof(YellowTeam), ComponentType.Exclude<TargetId>());
        m_blueTeamQuery = state.GetEntityQuery(typeof(BlueTeam));
        m_blueTeamIdleQuery = state.GetEntityQuery(typeof(BlueTeam), ComponentType.Exclude<TargetId>());
        m_resourcesQuery = state.GetEntityQuery(typeof(GridPosition));
    }

    public void OnDestroy(ref SystemState state)
    {
    }

    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<BeeConfig>();

        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
        var ecbWriter = ecb.AsParallelWriter();

        var allBlueBees = m_blueTeamQuery.ToEntityArray(Allocator.TempJob);
        var allYellowBees = m_yellowTeamQuery.ToEntityArray(Allocator.TempJob);

        var blueTeamJob = new TargetingEnemyJob()
        {
            ECB = ecbWriter,
            aggression = config.aggression,
            enemies = allYellowBees
        }.ScheduleParallel(m_blueTeamIdleQuery, state.Dependency);

        var yellowTeamJob = new TargetingEnemyJob()
        {
            ECB = ecbWriter,
            aggression = config.aggression,
            enemies = allBlueBees
        }.ScheduleParallel(m_yellowTeamIdleQuery, state.Dependency);

        var targetEnemyJob = JobHandle.CombineDependencies(blueTeamJob, yellowTeamJob);

        if (!m_resourcesQuery.IsEmpty)
        {
            new TargetingResourceJob()
            {
                ECB = ecbWriter,
                resources = m_resourcesQuery.ToEntityArray(Allocator.TempJob)
            }.ScheduleParallel(targetEnemyJob);

            // create hashsets for O(1) lookup in job
            var yellowHashSet = new NativeHashSet<Entity>();
            var blueHashSet = new NativeHashSet<Entity>();

            foreach (var e in allYellowBees)
                yellowHashSet.Add(e);
            foreach (var e in allBlueBees)
                blueHashSet.Add(e);

            // TODO revisit once we have resource pickup working...
            var yellowUpdatesJob = new TargetingUpdateJob()
            {
                ECB = ecb, friends = yellowHashSet, holders = SystemAPI.GetComponentLookup<Holder>(true)
            };
            yellowUpdatesJob.Schedule();

            var blueUpdatesJob = new TargetingUpdateJob()
            {
                ECB = ecb, friends = blueHashSet, holders = SystemAPI.GetComponentLookup<Holder>(true)
            };
            blueUpdatesJob.Schedule();
        }
    }
}