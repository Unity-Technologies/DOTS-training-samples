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
            ECB.SetComponentEnabled<IsAttacking>(idx, bee, true);
            ECB.SetComponentEnabled<IsHolding>(idx, bee, false);
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
        ECB.SetComponentEnabled<IsHolding>(idx, bee, true);
        ECB.SetComponentEnabled<IsAttacking>(idx, bee, false);
        ECB.SetComponent(idx, bee, new TargetId() { Value = resources[random.NextInt(0, resources.Length)] });
    }
}

[BurstCompile]
[WithNone(typeof(IsAttacking))]
[WithNone(typeof(IsHolding))]
partial struct TargetingUpdateJob : IJobEntity
{
    public EntityCommandBuffer ECB;

    [ReadOnly] public ComponentLookup<Holder> holders;
    [ReadOnly] public ComponentLookup<BlueTeam> blueTeamLookup;

    void Execute(Entity bee, in TargetId target)
    {
        if (holders.TryGetComponent(target.Value, out Holder holder) && blueTeamLookup.HasComponent(bee) != blueTeamLookup.HasComponent(holder.Value))
        {
            ECB.SetComponent(bee, new TargetId() { Value = holder.Value });
        }
    }
}

[BurstCompile]
partial struct DropTargetJob : IJobEntity {
    
    public EntityCommandBuffer.ParallelWriter ECB;

    [ReadOnly] public ComponentLookup<DecayTimer> deadLookup;

    void Execute(Entity entity, [ChunkIndexInQuery] int index, ref TargetId target) {
        if (target.Value == Entity.Null || deadLookup.HasComponent(target.Value)) {
            ECB.SetComponentEnabled<IsAttacking>(index, entity, false);
            ECB.SetComponentEnabled<IsHolding>(index, entity, false);
            ECB.SetComponentEnabled<TargetId>(index, entity, false);
        }
    }
}

[BurstCompile]
[UpdateAfter(typeof(BeeConstructionSystem))]
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

        m_yellowTeamQuery = state.GetEntityQuery(typeof(YellowTeam), ComponentType.Exclude<Decay>());
        m_yellowTeamIdleQuery = state.GetEntityQuery(typeof(YellowTeam), ComponentType.Exclude<TargetId>(), ComponentType.Exclude<Decay>());
        m_blueTeamQuery = state.GetEntityQuery(typeof(BlueTeam), ComponentType.Exclude<Decay>());
        m_blueTeamIdleQuery = state.GetEntityQuery(typeof(BlueTeam), ComponentType.Exclude<TargetId>(), ComponentType.Exclude<Decay>());
        m_resourcesQuery = state.GetEntityQuery(typeof(GridPosition), ComponentType.Exclude<Decay>(), ComponentType.Exclude<Falling>());
    }

    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<BeeConfig>();

        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();

        var allBlueBees = m_blueTeamQuery.ToEntityArray(Allocator.TempJob);
        var allYellowBees = m_yellowTeamQuery.ToEntityArray(Allocator.TempJob);
        
        var dropTargetJob = new DropTargetJob() {
            ECB = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter(),
            deadLookup = state.GetComponentLookup<DecayTimer>()
        }.ScheduleParallel(state.Dependency);
        
        var blueTeamJob = new TargetingEnemyJob()
        {
            ECB = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter(),
            aggression = config.aggression,
            enemies = allYellowBees
        }.ScheduleParallel(m_blueTeamIdleQuery, dropTargetJob);

        var yellowTeamJob = new TargetingEnemyJob()
        {
            ECB = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter(),
            aggression = config.aggression,
            enemies = allBlueBees
        }.ScheduleParallel(m_yellowTeamIdleQuery, dropTargetJob);

        var targetEnemyJob = JobHandle.CombineDependencies(blueTeamJob, yellowTeamJob);

        if (!m_resourcesQuery.IsEmpty)
        {
            var allResources = m_resourcesQuery.ToEntityArray(Allocator.TempJob);
            state.Dependency = new TargetingResourceJob()
            {
                ECB = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter(),
                resources = allResources
            }.ScheduleParallel(targetEnemyJob);
            
            allResources.Dispose(state.Dependency);
            
            // TODO revisit once we have resource pickup working...
            var yellowUpdatesJob = new TargetingUpdateJob()
            {
                ECB = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged), blueTeamLookup = state.GetComponentLookup<BlueTeam>(), holders = SystemAPI.GetComponentLookup<Holder>(true)
            };
            yellowUpdatesJob.Schedule();
        }
        else
        {
            state.Dependency = targetEnemyJob;
        }

        allBlueBees.Dispose(state.Dependency);
        allYellowBees.Dispose(state.Dependency);
    }
}
