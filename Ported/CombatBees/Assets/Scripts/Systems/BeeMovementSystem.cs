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
        var worldBoundsEntity = GetSingletonEntity<WorldBounds>();
        var bounds = GetComponent<WorldBounds>(worldBoundsEntity);
        var hiveBluePosition = bounds.AABB.Min +
                              new float3(bounds.HiveOffset / 2.0f, (bounds.AABB.Max.y - bounds.AABB.Min.y)/2.0f, (bounds.AABB.Max.z - bounds.AABB.Min.z)/2.0f);
        var hiveRedPosition = bounds.AABB.Max -
                              new float3(bounds.HiveOffset / 2.0f, (bounds.AABB.Max.y - bounds.AABB.Min.y)/2.0f, (bounds.AABB.Max.z - bounds.AABB.Min.z)/2.0f);
        // Update our target position
        Entities
            .WithReadOnly(transLookup)
            .WithAny<TeamRed, TeamBlue>()
            .ForEach((Entity beeEntity, ref Target target) =>
            {
                if (target.TargetType == TargetType.Food || target.TargetType == TargetType.Bee)
                {
                    if (transLookup.HasComponent(target.TargetEntity))
                        target.TargetPosition = transLookup[target.TargetEntity].Value;
                    else
                        target.Reset();
                }
            }).ScheduleParallel();
        
        var dtTime = Time.DeltaTime;
        Entity bloodPrefab = GetSingleton<ParticlePrefabs>().BloodPrefab;
        var foodPrefab = GetSingleton<FoodSpawner>().FoodPrefab;
        
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
                    else if (target.TargetType == TargetType.Food)
                    {
                        target.TargetType = TargetType.Hive;
                        target.TargetPosition = (HasComponent<TeamRed>(beeEntity) ? hiveRedPosition : hiveBluePosition);
                        ecb.DestroyEntity(entityInQueryIndex, target.TargetEntity);
                    }
                    else if (target.TargetType == TargetType.Hive)
                    {
                        //Dropping food.
                        target.Reset();
                        var instance = ecb.Instantiate(entityInQueryIndex, foodPrefab);
                        var foodInHive = new Translation {Value = translation.Value};
                        ecb.SetComponent(entityInQueryIndex, instance, foodInHive);
                        ecb.AddComponent(entityInQueryIndex, instance, new InHive{});
                    }
                }
            }).ScheduleParallel();
        
        system.AddJobHandleForProducer(Dependency);
    }
}
