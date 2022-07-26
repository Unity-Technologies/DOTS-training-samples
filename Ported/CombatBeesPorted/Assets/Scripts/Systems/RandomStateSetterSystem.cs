using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using Random = Unity.Mathematics.Random;

[BurstCompile]
partial struct RandomStateSetterJob : IJobEntity
{
    public EntityCommandBuffer Buffer;
    public float Aggressiveness;
    public NativeArray<Entity> EnemyTeam;
    public NativeArray<Entity> Resources;
    public uint RandomSeed;

    void Execute(Entity entity)
    {
        Random rand = Random.CreateFromIndex((uint)(entity.Index) + RandomSeed);
        if (rand.NextFloat() < Aggressiveness && Resources.Length > 0)
        {
            var resourceIndex = rand.NextInt(0, Resources.Length);
            Buffer.SetComponentEnabled<BeeStateIdle>(entity, false);
            Buffer.SetComponentEnabled<BeeStateGathering>(entity, true);
            Buffer.SetComponent(entity, new EntityOfInterest{ Value = Resources[resourceIndex] });
        }
        else if (EnemyTeam.Length > 0)
        {
            var enemyBeeIndex = rand.NextInt(0, EnemyTeam.Length);
            Buffer.SetComponentEnabled<BeeStateIdle>(entity, false);
            Buffer.SetComponentEnabled<BeeStateAttacking>(entity, true);
            Buffer.SetComponent(entity, new EntityOfInterest{ Value = EnemyTeam[enemyBeeIndex] });
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
        _resourceQuery = state.GetEntityQuery(new EntityQueryDesc
        {
            All = new ComponentType[] { },
            Any = new ComponentType[]
                { typeof(ResourceStateGrabbable), typeof(ResourceStateGrabbed), typeof(ResourceStateStacked) },
            None = new ComponentType[] { },
        });
        
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
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        var resources = _resourceQuery.ToEntityArray(Allocator.TempJob);
        var blueTeam = _blueTeamAllQuery.ToEntityArray(Allocator.TempJob);
        var yellowTeam = _yellowTeamAllQuery.ToEntityArray(Allocator.TempJob);
        
        // TODO - make this parallel
        var blueTeamRandomStateJob = new RandomStateSetterJob
        {
            Buffer = ecb,
            Aggressiveness = SystemAPI.GetSingleton<Config>().Aggressiveness,
            RandomSeed = (uint)UnityEngine.Time.frameCount,
            Resources =  resources,
            EnemyTeam =  yellowTeam,
        };
        blueTeamRandomStateJob.Schedule(_blueTeamIdleQuery);
        
        var yellowTeamRandomStateJob = new RandomStateSetterJob
        {
            Buffer = ecb,
            Aggressiveness = SystemAPI.GetSingleton<Config>().Aggressiveness,
            RandomSeed = (uint)UnityEngine.Time.frameCount,
            Resources =  resources,
            EnemyTeam =  blueTeam,
        };
        yellowTeamRandomStateJob.Schedule(_yellowTeamIdleQuery);
    }
}