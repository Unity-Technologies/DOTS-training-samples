using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

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