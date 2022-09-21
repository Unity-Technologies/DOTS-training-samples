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
            ECB.AddComponent<TargetId>(bee, new TargetId() { Value = resources[random.NextInt(0, resources.Length)] });
        }
    }
}

[BurstCompile]
partial struct TargetingSystem : ISystem
{
    private EntityQuery yellowTeam;
    private EntityQuery blueTeam;
    private EntityQuery resources;

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<BeeConfig>();

        yellowTeam = state.GetEntityQuery(typeof(YellowTeam));
        blueTeam = state.GetEntityQuery(typeof(BlueTeam));
        resources = state.GetEntityQuery(typeof(GridPosition));
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
            enemies = yellowTeam.ToEntityArray(Allocator.TempJob),
            resources = resources.ToEntityArray(Allocator.TempJob)
        };

        var yellowTeamJob = new TargetingJob()
        {
            ECB = ecb, random = Random.CreateFromIndex((uint)state.Time.ElapsedTime), aggression = config.aggression,
            enemies = blueTeam.ToEntityArray(Allocator.TempJob),
            resources = resources.ToEntityArray(Allocator.TempJob)
        };

        blueTeamJob.Schedule(blueTeam);
        yellowTeamJob.Schedule(yellowTeam);
    }
}