using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[BurstCompile]
[WithNone(typeof(TargetId))]
partial struct TargetingJob : IJobEntity
{
    public EntityCommandBuffer ECB;

    public Random random;
    public float aggression;

    [ReadOnly] public NativeArray<Entity> enemies;
    [ReadOnly] public NativeArray<Entity> resources;

    void Execute(Entity bee)
    {
        if (random.NextFloat() < aggression && enemies.Length > 0)
        {
            // attacking
            ECB.AddComponent<TargetId>(bee, new TargetId() { Value = enemies[random.NextInt(0, enemies.Length)] });
        }
        else if (resources.Length > 0)
        {
            // gathering
            // TODO for now we are just grabbing a random resource, but the Original sample only sets this target if it is on the top of the stack 
            ECB.AddComponent<TargetId>(bee, new TargetId() { Value = resources[random.NextInt(0, resources.Length)] });
        }
    }
}

[BurstCompile]
partial struct TargetingSystem : ISystem
{
    private EntityQuery m_yellowTeamQuery;
    private EntityQuery m_blueTeamQuery;
    private EntityQuery m_resourcesQuery;

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<BeeConfig>();

        m_yellowTeamQuery = state.GetEntityQuery(typeof(YellowTeam));
        m_blueTeamQuery = state.GetEntityQuery(typeof(BlueTeam));
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

        var blueTeamJob = new TargetingJob()
        {
            ECB = ecb, random = Random.CreateFromIndex((uint)state.Time.ElapsedTime), aggression = config.aggression,
            enemies = m_yellowTeamQuery.ToEntityArray(Allocator.TempJob),
            resources = m_resourcesQuery.ToEntityArray(Allocator.TempJob)
        };

        var yellowTeamJob = new TargetingJob()
        {
            ECB = ecb, random = Random.CreateFromIndex((uint)state.Time.ElapsedTime), aggression = config.aggression,
            enemies = m_blueTeamQuery.ToEntityArray(Allocator.TempJob),
            resources = m_resourcesQuery.ToEntityArray(Allocator.TempJob)
        };

        blueTeamJob.Schedule(m_blueTeamQuery);
        yellowTeamJob.Schedule(m_yellowTeamQuery);
    }
}