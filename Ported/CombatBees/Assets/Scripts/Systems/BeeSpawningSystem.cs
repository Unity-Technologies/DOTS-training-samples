using Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

[BurstCompile]
public partial struct BeeSpawningSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<BeeConfig>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var Random = Unity.Mathematics.Random.CreateFromIndex(98);
        
        var beeConfigEntity = SystemAPI.GetSingletonEntity<BeeConfig>();
        var beeConfig = SystemAPI.GetComponent<BeeConfig>(beeConfigEntity);
        
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        var Team1SpawnJob = new SpawnBeeJob
        {
            ECB = ecb,
            BeePrefab = beeConfig.BeePrefab,
            BeesToSpawn = beeConfig.BeesToSpawn,
            Random = Random,
            Team = beeConfig.Team1
        };
        
        var Team2SpawnJob = new SpawnBeeJob
        {
            ECB = ecb,
            BeePrefab = beeConfig.BeePrefab,
            BeesToSpawn = beeConfig.BeesToSpawn,
            Random = Random,
            Team = beeConfig.Team2
        };
        
        var team1Handle = Team1SpawnJob.Schedule(state.Dependency);
        var team2Handle = Team2SpawnJob.Schedule(team1Handle);
        team2Handle.Complete();
        
        state.Enabled = false;
    }
}

[BurstCompile]
public struct SpawnBeeJob : IJob
{
    public EntityCommandBuffer ECB;
    public Entity BeePrefab;
    public int BeesToSpawn;
    public Team Team;
    public Random Random;
    
    public void Execute()
    {
        var bees = CollectionHelper.CreateNativeArray<Entity>(BeesToSpawn, Allocator.Temp);
        ECB.Instantiate(BeePrefab, bees);

        foreach (var bee in bees)
        {
            var uniformScaleTransform = new UniformScaleTransform
            {
                Position = Random.NextFloat3(Team.MinBounds, Team.MaxBounds),
                Rotation = quaternion.identity,
                Scale = Random.NextFloat()
            };
            ECB.SetComponent(bee, new LocalToWorldTransform
            {
                Value = uniformScaleTransform
            });
            ECB.SetComponent(bee, new URPMaterialPropertyBaseColor
            {
                Value = Team.Color
            });
        }
    }
}
