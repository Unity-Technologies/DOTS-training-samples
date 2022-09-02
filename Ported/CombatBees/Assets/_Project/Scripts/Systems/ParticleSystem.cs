using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public partial class ParticleSystem : SystemBase
{
    
    private EntityCommandBufferSystem ECBSystem;

    protected override void OnCreate()
    {
        base.OnCreate();
        ECBSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {        
        if (!HasSingleton<GameGlobalData>() || 
            !HasSingleton<GameRuntimeData>())
            return;

        EntityCommandBuffer ecb;
        EntityCommandBuffer.ParallelWriter ecbParallel;
        float deltaTime = Time.DeltaTime;
        float time = (float)Time.ElapsedTime;
        GameGlobalData globalData = GetSingleton<GameGlobalData>();
        Entity runtimeDataEntity = GetSingletonEntity<GameRuntimeData>();
        ComponentDataFromEntity<GameRuntimeData> runtimeDataFromEntity = GetComponentDataFromEntity<GameRuntimeData>(false);
        GridCharacteristics gridCharacteristics = GetSingleton<GameRuntimeData>().GridCharacteristics;

        // Particle spawning
        ecb = ECBSystem.CreateCommandBuffer();
        Entities
            .WithName("ParticleSpawnJob")
            .ForEach((Entity entity, ref DynamicBuffer<ParticleSpawnEvent> spawnEvents) =>
            {
                GameRuntimeData runtimeData = runtimeDataFromEntity[runtimeDataEntity];
                
                for (int i = 0; i < spawnEvents.Length; i++)
                {
                    ParticleSpawnEvent evnt = spawnEvents[i];

                    Entity particlePrefab = default;
                    switch(evnt.ParticleType)
                    {
                        case ParticleType.Spark:
                            particlePrefab = globalData.ParticleSparkPrefab;
                            break;
                        case ParticleType.Smoke:
                            particlePrefab = globalData.ParticleSmokePrefab;
                            break;
                        case ParticleType.BloodInFlight:
                            particlePrefab = globalData.ParticleBloodInFlightPrefab;
                            break;
                        case ParticleType.BloodSettled:
                            particlePrefab = globalData.ParticleBloodSettledPrefab;
                            break;
                    }

                    Entity particleInstance = ecb.Instantiate(particlePrefab);
                    ecb.SetComponent(particleInstance, new Translation { Value = evnt.Position });
                    ecb.SetComponent(particleInstance, new Rotation { Value = evnt.Rotation });

                    if (HasComponent<ParticleVelocity>(particlePrefab))
                    {
                        float3 velDirection = GameUtilities.GetRandomInArcSphere(0f, evnt.VelocityDirectionRandomizationAngles, evnt.VelocityDirection, ref runtimeData.Random);
                        float p = evnt.VelocityMagnitudeRandomization * evnt.VelocityMagnitude;
                        float velMagnitude = runtimeData.Random.NextFloat(evnt.VelocityMagnitude - p, evnt.VelocityMagnitude + p);
                        ecb.SetComponent(particleInstance, new ParticleVelocity() { Velocity = velDirection * velMagnitude });
                    }

                    if (evnt.LifetimeRandomization > 0f && HasComponent<ParticleLifetime>(particlePrefab))
                    {
                        ParticleLifetime lifetime = GetComponent<ParticleLifetime>(particlePrefab);
                        float p = lifetime.MaxLifetime * evnt.LifetimeRandomization;
                        lifetime.MaxLifetime = runtimeData.Random.NextFloat(lifetime.MaxLifetime - p, lifetime.MaxLifetime + p);
                        ecb.SetComponent(particleInstance, lifetime);
                    }
                    
                    if (evnt.SizeRandomization > 0f && HasComponent<ParticleSize>(particlePrefab))
                    {
                        ParticleSize size = GetComponent<ParticleSize>(particlePrefab);
                        float3 p = size.Size * evnt.SizeRandomization;
                        size.Size = runtimeData.Random.NextFloat3(size.Size - p, size.Size + p);
                        ecb.SetComponent(particleInstance, size);
                    }
                }
                spawnEvents.Clear();

                runtimeDataFromEntity[runtimeDataEntity] = runtimeData;
            }).Schedule();

        ecbParallel = ECBSystem.CreateCommandBuffer().AsParallelWriter();
        Entities
            .WithName("ParticleLifetimeJob")
            .ForEach((Entity entity, int entityInQueryIndex, ref ParticleLifetime lifetime) =>
            {
                lifetime.Lifetime += deltaTime;
                if (lifetime.Lifetime > lifetime.MaxLifetime)
                {
                    ecbParallel.DestroyEntity(entityInQueryIndex, entity);
                }
            }).ScheduleParallel();

        Entities
            .WithName("ParticleGravityJob")
            .ForEach((ref ParticleVelocity vel, ref ParticleGravity gravity) =>
            {
                vel.Velocity += gravity.Value * deltaTime;
            }).ScheduleParallel();

        Entities
            .WithName("ParticleDragJob")
            .ForEach((ref ParticleVelocity vel, ref ParticleDrag drag) =>
            {
                vel.Velocity *= (1f - (drag.Drag * deltaTime));
            }).ScheduleParallel();

        Entities
            .WithName("ParticleVelocityJob")
            .ForEach((ref ParticleVelocity vel, ref Translation translation) =>
            {
                translation.Value += vel.Velocity * deltaTime;
            }).ScheduleParallel();

        Entities
            .WithName("ParticleOrientTowardsVelocityJob")
            .ForEach((ref Rotation Rotation, in ParticleVelocity vel, in ParticleOrientTowardsVelocity orient) =>
            {
                Rotation.Value = quaternion.LookRotationSafe(vel.Velocity, math.up());
            }).ScheduleParallel();

        Entities
            .WithName("ParticleSizeJob")
            .ForEach((ref NonUniformScale scale, in ParticleSize size) =>
            {
                scale.Value = size.Size;
            }).ScheduleParallel();

        Entities
            .WithName("ParticleShrinkOverLifetimeJob")
            .ForEach((ref NonUniformScale scale, in ParticleShrinkOverLifetime shrinkOverLifetime, in ParticleLifetime lifetime) =>
            {
                float lifetimeRatio = math.clamp(lifetime.Lifetime / lifetime.MaxLifetime, 0f, 1f);
                scale.Value *= (1f - lifetimeRatio);
            }).ScheduleParallel();
        
        Entities
            .WithName("ParticleStretchWithVelocityJob")
            .ForEach((ref NonUniformScale scale, in ParticleVelocity vel, in ParticleStretchWithVelocity stretch) =>
            {
                scale.Value.z *= math.clamp(1f + (stretch.Factor * math.length(vel.Velocity)), 1f, stretch.Max);
            }).ScheduleParallel();

        ecb = ECBSystem.CreateCommandBuffer();
        Entities
            .WithName("ParticleBloodInFlightJob")
            .ForEach((Entity entity, ref ParticleBloodInFlight bloodParticle, in Translation translation) =>
            {
                if (!gridCharacteristics.LevelBounds.IsInside(translation.Value))
                {
                    DynamicBuffer<ParticleSpawnEvent> particleEventsBuffer = GetBuffer<ParticleSpawnEvent>(runtimeDataEntity);
                    float3 pointOnBounds = gridCharacteristics.LevelBounds.GetClosestPoint(translation.Value);
                    particleEventsBuffer.Add(new ParticleSpawnEvent
                    {
                        ParticleType = ParticleType.BloodSettled,
                        Position = pointOnBounds,
                        Rotation = quaternion.LookRotationSafe(math.normalizesafe(pointOnBounds - translation.Value), math.normalizesafe(new float3(1f))),
                        LifetimeRandomization = 0.3f,
                        SizeRandomization = 0.3f,
                    });
                    
                    ecb.DestroyEntity(entity);
                }
            }).Schedule();
        
        ECBSystem.AddJobHandleForProducer(Dependency);
    }
}
