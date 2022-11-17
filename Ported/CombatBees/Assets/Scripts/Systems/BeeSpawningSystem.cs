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
        var random = Random.CreateFromIndex(98);
        
        var beeConfigEntity = SystemAPI.GetSingletonEntity<BeeConfig>();
        var beeConfig = SystemAPI.GetComponent<BeeConfig>(beeConfigEntity);
        
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        var team1SpawnJob = new SpawnBeeJob
        {
            ECB = ecb.AsParallelWriter(),
            Stretch = beeConfig.Stretch,
            BeePrefab = beeConfig.BeePrefab,
            BeesToSpawn = beeConfig.BeesToSpawn,
            MaxBeeSize = beeConfig.MaxBeeSize,
            MinBeeSize = beeConfig.MinBeeSize,
            Random = random,
            Team = beeConfig.Team1
        };
        
        var team2SpawnJob = new SpawnBeeJob
        {
            ECB = ecb.AsParallelWriter(),
            Stretch = beeConfig.Stretch,
            BeePrefab = beeConfig.BeePrefab,
            BeesToSpawn = beeConfig.BeesToSpawn,
            MaxBeeSize = beeConfig.MaxBeeSize,
            MinBeeSize = beeConfig.MinBeeSize,
            Random = random,
            Team = beeConfig.Team2
        };

        var team1Handle = team1SpawnJob.ScheduleParallel(state.Dependency);
        state.Dependency = team2SpawnJob.ScheduleParallel(team1Handle);
        
        state.Enabled = false;
    }
}

[BurstCompile]
partial struct SpawnBeeJob : IJobEntity
{
    public EntityCommandBuffer.ParallelWriter ECB;
    public Team Team;
    public Random Random;
    public Entity BeePrefab;
    public int BeesToSpawn;
    public float MinBeeSize;
    public float MaxBeeSize;
    public float Stretch;
    
    public void Execute([ChunkIndexInQuery] int index)
    {
        var bees = CollectionHelper.CreateNativeArray<Entity>(BeesToSpawn, Allocator.Temp);
        ECB.Instantiate(index, BeePrefab, bees);

        foreach (var bee in bees)
        {
            var uniformScaleTransform = new UniformScaleTransform
            {
                Position = Random.NextFloat3(Team.MinBounds, Team.MaxBounds),
                Rotation = quaternion.identity,
                Scale = Random.NextFloat(MinBeeSize, MaxBeeSize)
            };
            ECB.SetComponent(index, bee, new LocalToWorldTransform
            {
                Value = uniformScaleTransform
            });
            ECB.AddComponent(index, bee, new PostTransformMatrix());
            ECB.SetComponent(index, bee, new URPMaterialPropertyBaseColor
            {
                Value = Team.Color
            });
            ECB.SetComponent(index, bee, new Bee
            {
                Scale = uniformScaleTransform.Scale,
                Team = Team
            });
            ECB.AddComponent(index, bee, new Physical
            {
                Position = uniformScaleTransform.Position,
                Velocity = float3.zero,
                IsFalling = false,
                Collision = Physical.FieldCollisionType.Bounce,
                Stretch = Stretch,
            });
            ECB.AddSharedComponent(index, bee, new TeamIdentifier
            {
                TeamNumber = Team.TeamNumber
            });
            
            ECB.AddComponent(index, bee, new Dead()
            {
                DeathTimer = 2f
            });
            
            ECB.SetComponentEnabled<Dead>(index, bee, false);
        }
    }
}
