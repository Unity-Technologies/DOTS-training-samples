using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using Random = Unity.Mathematics.Random;

[BurstCompile]
partial struct RandomStateSetterJob : IJobEntity
{
    public EntityCommandBuffer.ParallelWriter ECB;
    public float Aggressiveness;
    [ReadOnly] public NativeArray<Entity> EnemyTeam;
    [ReadOnly] public NativeArray<Entity> Resources;
    public uint RandomSeed;

    void Execute([ChunkIndexInQuery] int chunkIndex, Entity entity)
    {
        Random rand = Random.CreateFromIndex((uint)(entity.Index) + RandomSeed);
        if (rand.NextFloat() > Aggressiveness && Resources.Length > 0)
        {
            var resourceIndex = rand.NextInt(0, Resources.Length);
            ECB.SetComponentEnabled<BeeStateIdle>(chunkIndex, entity, false);
            ECB.SetComponentEnabled<BeeStateGathering>(chunkIndex,entity, true);
            ECB.SetComponent(chunkIndex, entity, new EntityOfInterest{ Value = Resources[resourceIndex] });
        }
        else if (EnemyTeam.Length > 0)
        {
            var enemyBeeIndex = rand.NextInt(0, EnemyTeam.Length);
            ECB.SetComponentEnabled<BeeStateIdle>(chunkIndex, entity, false);
            ECB.SetComponentEnabled<BeeStateAttacking>(chunkIndex, entity, true);
            ECB.SetComponent(chunkIndex, entity, new EntityOfInterest{ Value = EnemyTeam[enemyBeeIndex] });
        }
    }
}

[BurstCompile]
public partial struct RandomStateSetterSystem : ISystem
{
    private EntityQuery _blueTeamAllQuery;
    private EntityQuery _blueTeamIdleQuery;

    private EntityQuery _yellowTeamAllQuery;
    private EntityQuery _yellowTeamIdleQuery;

    private EntityQuery _resourceQuery;
    
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Config>();

        _resourceQuery = state.GetEntityQuery(typeof(Resource));
        
        _blueTeamAllQuery = state.GetEntityQuery(typeof(BlueTeam));
        _blueTeamIdleQuery = state.GetEntityQuery(typeof(BlueTeam), typeof(BeeStateIdle));
        
        _yellowTeamAllQuery = state.GetEntityQuery(typeof(YellowTeam));
        _yellowTeamIdleQuery = state.GetEntityQuery(typeof(YellowTeam), typeof(BeeStateIdle));
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // Creating an EntityCommandBuffer to defer the structural changes required by instantiation.
        var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();

        var resources = _resourceQuery.ToEntityArray(state.WorldUpdateAllocator);
        var blueTeam = _blueTeamAllQuery.ToEntityArray(state.WorldUpdateAllocator);
        var yellowTeam = _yellowTeamAllQuery.ToEntityArray(state.WorldUpdateAllocator);
        
        var blueTeamRandomStateJob = new RandomStateSetterJob
        {
            ECB = ecb,
            Aggressiveness = SystemAPI.GetSingleton<Config>().Aggressiveness,
            RandomSeed = (uint)Time.frameCount,
            Resources =  resources,
            EnemyTeam =  yellowTeam,
        };
        blueTeamRandomStateJob.ScheduleParallel(_blueTeamIdleQuery);
        
        var yellowTeamRandomStateJob = new RandomStateSetterJob
        {
            ECB = ecb,
            Aggressiveness = SystemAPI.GetSingleton<Config>().Aggressiveness,
            RandomSeed = (uint)Time.frameCount,
            Resources =  resources,
            EnemyTeam =  blueTeam,
        };
        yellowTeamRandomStateJob.ScheduleParallel(_yellowTeamIdleQuery);
    }
}