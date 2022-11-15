using Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using Random = Unity.Mathematics.Random;

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

        var team1SpawnJob = new SpawnBeeJob
        {
            ECB = ecb,
            BeeConfig = beeConfig,
            Random = Random,
            Team = beeConfig.Team1
        };
        
        var team2SpawnJob = new SpawnBeeJob
        {
            ECB = ecb,
            BeeConfig = beeConfig,
            Random = Random,
            Team = beeConfig.Team2
        };

        var team1Handle = team1SpawnJob.Schedule(state.Dependency);
        var team2Handle = team2SpawnJob.Schedule(team1Handle);
        team2Handle.Complete();
        
        state.Enabled = false;
    }
}

[BurstCompile]
public struct SpawnBeeJob : IJob
{
    public EntityCommandBuffer ECB;
    public Team Team;
    public Random Random;
    public BeeConfig BeeConfig;
    
    public void Execute()
    {
        var bees = CollectionHelper.CreateNativeArray<Entity>(BeeConfig.BeesToSpawn, Allocator.Temp);
        ECB.Instantiate(BeeConfig.BeePrefab, bees);

        foreach (var bee in bees)
        {
            var uniformScaleTransform = new UniformScaleTransform
            {
                Position = Random.NextFloat3(Team.MinBounds, Team.MaxBounds),
                Rotation = quaternion.identity,
                Scale = Random.NextFloat(BeeConfig.MinBeeSize, BeeConfig.MaxBeeSize)
            };
            ECB.SetComponent(bee, new LocalToWorldTransform
            {
                Value = uniformScaleTransform
            });
            ECB.SetComponent(bee, new URPMaterialPropertyBaseColor
            {
                Value = Team.Color
            });
            ECB.SetComponent(bee, new Bee
            {
                Scale = uniformScaleTransform.Scale,
                Team = Team
            });
            ECB.AddComponent(bee, new Physical
            {
                Position = uniformScaleTransform.Position
            });
            ECB.AddSharedComponent(bee, new TeamIdentifier
            {
                TeamNumber = Team.TeamNumber
            });
            
            ECB.AddComponent(bee, new Dead()
            {
                DeathTimer = 2f
            });
            
            ECB.SetComponentEnabled<Dead>(bee, false);

        }
    }
}
