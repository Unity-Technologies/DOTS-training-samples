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

        var ecb1 = m_ECBSystem.CreateCommandBuffer().ToConcurrent();
        var ecb2 = m_ECBSystem.CreateCommandBuffer().ToConcurrent();
        var random = m_Random;
        var dt = Time.DeltaTime;

        // Particles
        Entities
            .WithNone<TeamOne, TeamTwo, ResourceEntity>()
            .ForEach((int entityInQueryIndex, Entity entity, ref DespawnTimer timer) =>
            {
                timer.Time -= dt;
                if (timer.Time <= 0)
                    ecb1.DestroyEntity(entityInQueryIndex, entity);
            }).ScheduleParallel();

        var despawnTimers = GetComponentDataFromEntity<DespawnTimer>();

        // Bees
        var job1 = Entities
            .WithAny<TeamOne, TeamTwo>()
            .WithAll<DespawnTimer>()
            .WithNativeDisableContainerSafetyRestriction(despawnTimers)
            .WithNativeDisableContainerSafetyRestriction(particleSpawners)
            .ForEach((int entityInQueryIndex, Entity entity, in LocalToWorld ltw, in Velocity v) =>
            {
                var timer = despawnTimers[entity];
                timer.Time -= dt;

                if (timer.Time <= 0)
                {
                    var target = GetComponent<Target>(entity);
                    if (target.ResourceTarget != Entity.Null)
                    {
                        ecb1.RemoveComponent<Carried>(entityInQueryIndex, target.ResourceTarget);
                    }

                    if (particleSpawners.Length > 0)
                    {
                        var spawner = particleSpawners[0];

                        for (var i = 0; i < spawner.NumBloodParticles; ++i)
                        {
                            var particle = ecb1.Instantiate(entityInQueryIndex, spawner.BloodPrefab);
                            ecb1.SetComponent(entityInQueryIndex, particle, new Translation { Value = ltw.Position });
                            ecb1.SetComponent(entityInQueryIndex, particle, new Velocity { Value = v.Value + random.NextFloat3Direction() });
                            ecb1.AddComponent<Gravity>(entityInQueryIndex, particle);
                            ecb1.AddComponent(entityInQueryIndex, particle, new DespawnTimer { Time = random.NextFloat(2f, 5f) });
                        }
                    }

                    ecb1.DestroyEntity(entityInQueryIndex, entity);
                }

                despawnTimers[entity] = timer;
            }).ScheduleParallel(Dependency);

        // Resources 
        var job2 = Entities
            .WithAll<ResourceEntity, DespawnTimer>()
            .WithNone<TeamOne, TeamTwo>()
            .WithNativeDisableContainerSafetyRestriction(despawnTimers)
            .WithNativeDisableContainerSafetyRestriction(particleSpawners)
            .ForEach((int entityInQueryIndex, Entity entity, in LocalToWorld ltw) =>
            {
                var timer = despawnTimers[entity];
                timer.Time -= dt;

                if (timer.Time <= 0)
                {
                    if (particleSpawners.Length > 0)
                    {
                        var spawner = particleSpawners[0];

                        var particle = ecb2.Instantiate(entityInQueryIndex, spawner.SmokePrefab);
                        ecb2.SetComponent(entityInQueryIndex, particle, new Translation { Value = ltw.Position });
                        ecb2.SetComponent(entityInQueryIndex, particle, new Velocity { Value = new float3(0, random.NextFloat(1f, 5f), 0) });
                        ecb2.AddComponent(entityInQueryIndex, particle, new DespawnTimer { Time = random.NextFloat(0.2f, 0.35f) });
                    }

                    ecb2.DestroyEntity(entityInQueryIndex, entity);
                }

                despawnTimers[entity] = timer;
            }).ScheduleParallel(Dependency);

        Dependency = JobHandle.CombineDependencies(job1, job2);

        particleSpawners.Dispose(Dependency);

        m_ECBSystem.AddJobHandleForProducer(Dependency);
    }
}
