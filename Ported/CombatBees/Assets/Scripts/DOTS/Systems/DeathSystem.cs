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
        var dt = Time.DeltaTime;

        // Particles
        Entities
            .WithNone<TeamOne, TeamTwo, ResourceEntity>()
            .ForEach((int entityInQueryIndex, Entity entity, ref DespawnTimer timer) =>
            {
                timer.Time -= dt;
                if (timer.Time <= 0)
                    ecb.DestroyEntity(entityInQueryIndex, entity);
            }).ScheduleParallel();

        // Resources 
        Entities
            .WithAll<ResourceEntity>()
            .WithNone<TeamOne, TeamTwo>()
            .ForEach((int entityInQueryIndex, Entity entity, ref DespawnTimer timer, in LocalToWorld ltw) =>
            {
                if (particleSpawners.Length > 0)
                {
                    var spawner = particleSpawners[0];

                    var particle = ecb.Instantiate(entityInQueryIndex, spawner.SmokePrefab);
                    ecb.SetComponent(entityInQueryIndex, particle, new Translation { Value = ltw.Position });
                    ecb.SetComponent(entityInQueryIndex, particle, new Velocity { Value = new float3(0, random.NextFloat(1f, 5f), 0) });
                    ecb.AddComponent(entityInQueryIndex, particle, new DespawnTimer { Time = random.NextFloat(0.2f, 0.35f) });
                }
                timer.Time -= dt;
                if (timer.Time <= 0)
                    ecb.DestroyEntity(entityInQueryIndex, entity);
            }).ScheduleParallel(); 

        // Bees
        Entities
            .WithAny<TeamOne, TeamTwo>()
            .WithDeallocateOnJobCompletion(particleSpawners)
            .ForEach((int entityInQueryIndex, Entity entity, ref DespawnTimer timer, in LocalToWorld ltw, in Velocity v) =>
            {
                if (particleSpawners.Length > 0)
                {
                    var spawner = particleSpawners[0];

                    var particle = ecb.Instantiate(entityInQueryIndex, spawner.BloodPrefab);
                    ecb.SetComponent(entityInQueryIndex, particle, new Translation { Value = ltw.Position });
                    ecb.SetComponent(entityInQueryIndex, particle, new Velocity { Value = v.Value }); 
                }
                timer.Time -= dt;
                if (timer.Time <= 0)
                    ecb.DestroyEntity(entityInQueryIndex, entity);
            }).ScheduleParallel();

        m_ECBSystem.AddJobHandleForProducer(Dependency);
    }
}
