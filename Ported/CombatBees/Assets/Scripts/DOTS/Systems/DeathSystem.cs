using System;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Random = Unity.Mathematics.Random;

[UpdateInGroup(typeof(LateSimulationSystemGroup))]
public class DeathSystem : SystemBase
{
    private EntityQuery m_ParticleSpawnerQuery;

    private EntityCommandBufferSystem m_ECBSystem;
    private Random m_Random;


    protected override void OnCreate()
    {
        m_ParticleSpawnerQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new[]
            {
                ComponentType.ReadOnly<ParticleSpawnerAuthoring>()
            }
        });

        m_ECBSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        m_Random = new Random((uint)DateTime.Now.Ticks);
    }

    protected override void OnUpdate()
    {
        var particleSpawners = m_ParticleSpawnerQuery.ToComponentDataArrayAsync<ParticleSpawnerAuthoring>(Unity.Collections.Allocator.TempJob, out var particleSpawnersHandle);

        Dependency = JobHandle.CombineDependencies(Dependency, particleSpawnersHandle);

        var ecb = m_ECBSystem.CreateCommandBuffer().ToConcurrent();
        var random = m_Random; 

        // Resources 
        Entities
            .WithAll<Dead>()
            .WithNone<TeamOne, TeamTwo>()
            .ForEach((int entityInQueryIndex, Entity entity, in LocalToWorld ltw) =>
            {
                if (particleSpawners.Length > 0)
                {
                    var spawner = particleSpawners[0];

                    var particle = ecb.Instantiate(entityInQueryIndex, spawner.SmokePrefab);
                    ecb.SetComponent(entityInQueryIndex, particle, new Translation { Value = ltw.Position });
                    ecb.SetComponent(entityInQueryIndex, particle, new Velocity { Value = new float3(0, random.NextFloat(1f, 5f), 0) });
                    ecb.AddComponent(entityInQueryIndex, particle, new DespawnTimer { Time = random.NextFloat(0.2f, 0.35f) });
                }

                ecb.DestroyEntity(entityInQueryIndex, entity);
            }).ScheduleParallel(); 

        // Bees
        Entities.WithAll<Dead>()
            .WithAny<TeamOne, TeamTwo>()
            .WithDeallocateOnJobCompletion(particleSpawners)
            .ForEach((int entityInQueryIndex, Entity entity, in LocalToWorld ltw) =>
            {
                if (particleSpawners.Length > 0)
                {
                    var spawner = particleSpawners[0];
                    //spawner.
                    //ecb.Instantiate(entityInQueryIndex, )
                }

                ecb.DestroyEntity(entityInQueryIndex, entity);
            }).ScheduleParallel();

        m_ECBSystem.AddJobHandleForProducer(Dependency);
    }
}
