using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

[BurstCompile]
partial struct TargetingEnemyJob : IJobEntity
{
    public EntityCommandBuffer.ParallelWriter ECB;

    public Random random;
    public float aggression;

    [ReadOnly] public NativeArray<Entity> enemies;

    void Execute(Entity bee, [EntityInQueryIndex] int idx)
    {
        if (enemies.Length == 0)
        {
            return;
        }
        
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

    public Random random;

    [ReadOnly] public NativeArray<Entity> resources;

    void Execute(Entity bee, [EntityInQueryIndex] int idx)
    {
        ECB.SetComponentEnabled<TargetId>(idx, bee, true);
        ECB.SetComponent(idx, bee, new TargetId() { Value = resources[random.NextInt(0, resources.Length)] });
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

        var blueTeamJob = new TargetingEnemyJob()
        {
            ECB = ecbWriter, random = Random.CreateFromIndex((uint)state.Time.ElapsedTime),
            aggression = config.aggression,
            enemies = m_yellowTeamQuery.ToEntityArray(Allocator.TempJob)
        }.ScheduleParallel(m_blueTeamIdleQuery, state.Dependency);

        var yellowTeamJob = new TargetingEnemyJob()
        {
            ECB = ecbWriter, random = Random.CreateFromIndex((uint)state.Time.ElapsedTime),
            aggression = config.aggression,
            enemies = m_blueTeamQuery.ToEntityArray(Allocator.TempJob)
        }.ScheduleParallel(m_yellowTeamIdleQuery, state.Dependency);

        var enemyJob = JobHandle.CombineDependencies(blueTeamJob, yellowTeamJob);

        if (!m_resourcesQuery.IsEmpty)
        {
            new TargetingResourceJob()
            {
                ECB = ecbWriter, random = Random.CreateFromIndex((uint)state.Time.ElapsedTime),
                resources = m_resourcesQuery.ToEntityArray(Allocator.TempJob)
            }.ScheduleParallel(enemyJob);
        }
    }
}