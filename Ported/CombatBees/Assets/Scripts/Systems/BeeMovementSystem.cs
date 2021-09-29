using Unity.Entities;
using Unity.Collections;
using Unity.Entities.CodeGeneratedJobForEach;
using Unity.Transforms;
using Unity.Mathematics;

public partial class BeeMovementSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var transLookup = GetComponentDataFromEntity<Translation>(true);

        // Update our target position
        Entities
            .WithReadOnly(transLookup)
            .WithAny<TeamRed, TeamBlue>()
            .ForEach((Entity beeEntity, ref Target target) =>
            {
                if (target.TargetType != TargetType.None && transLookup.HasComponent(target.TargetEntity))
                {
                    target.TargetPosition = transLookup[target.TargetEntity].Value;
                }
            }).ScheduleParallel();
        
        var dtTime = Time.DeltaTime;
        Entity bloodPrefab = GetSingleton<ParticlePrefabs>().BloodPrefab;
        float3 up = new float3( 0.0f, 1.0f, 0.0f );
        var system = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        var ecb = system.CreateCommandBuffer().AsParallelWriter();
        var particleArchetype = EntityManager.CreateArchetype(typeof(ParticleSpawner));
        
        // Movement Update
        Entities
            .WithAny<TeamRed, TeamBlue>()
            .ForEach((Entity beeEntity, int entityInQueryIndex, ref Translation translation, ref Target target) =>
            {
                translation.Value += (math.normalize(target.TargetPosition - translation.Value) * 5.0f * dtTime);

                float distanceSqToTarget = math.distancesq(target.TargetPosition, translation.Value);

                if (distanceSqToTarget <= 4.0f)
                {
                    if (target.TargetType == TargetType.Bee)
                    {
                        // spawn blood
                        var bloodSpawnerEntity = ecb.CreateEntity(entityInQueryIndex, particleArchetype);
                        var bloodSpawner = new ParticleSpawner
                        {
                            Prefab = bloodPrefab,
                            Position = translation.Value,
                            Direction = up,
                            Spread = 0.2f,
                            Lifetime = 10.0f,
                            Count = 5,
                        };
                        ecb.SetComponent(entityInQueryIndex, bloodSpawnerEntity, bloodSpawner);
                        
                        // destroy and reset target
                        ecb.DestroyEntity(entityInQueryIndex, target.TargetEntity);
                        target.Reset();
                    }
                }
            }).ScheduleParallel();
        
        system.AddJobHandleForProducer(Dependency);
    }
}
