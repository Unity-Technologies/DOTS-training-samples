using Unity.Entities;
using Unity.Collections;
using Unity.Entities.CodeGeneratedJobForEach;
using Unity.Transforms;
using Unity.Mathematics;

public partial class BeeMovementSystem : SystemBase
{
    private Random random;
    protected override void OnCreate()
    {
        base.OnCreate();
        random = new Random((uint)System.DateTime.Now.Ticks);
    }
    
    protected override void OnUpdate()
    {
        var transLookup = GetComponentDataFromEntity<Translation>(true);
        var bounds = GetSingleton<WorldBounds>();
        Constants constants = GetSingleton<Constants>();
        uint seed = random.NextUInt();
 
        // Update our target position
        Entities
            .WithReadOnly(transLookup)
            .WithAny<TeamRed, TeamBlue>()
            .ForEach((Entity beeEntity, int entityInQueryIndex, ref Target target) =>
            {
                if (target.TargetType == TargetType.Food || target.TargetType == TargetType.Bee)
                {
                    if (transLookup.HasComponent(target.TargetEntity))
                    {
                        target.TargetPosition = transLookup[target.TargetEntity].Value;
                    }
                    else
                    {
                        target.Reset();
                        target.TargetType = TargetType.Wander;
                    }
                }

                if (target.TargetType == TargetType.Wander && math.all(target.TargetPosition == float3.zero))
                {
                    var random = new Random((uint)entityInQueryIndex + seed);
                    target.TargetPosition = WorldUtils.GetRandomInBoundsPosition(bounds, ref random);
                }
            }).ScheduleParallel();
        
        var dtTime = Time.DeltaTime;
        Prefabs prefabs = GetSingleton<Prefabs>();

        var system = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        var ecb = system.CreateCommandBuffer().AsParallelWriter();
        var particleArchetype = EntityManager.CreateArchetype(typeof(ParticleSpawner));
        
        // Movement Update
        Entities
            .WithAny<TeamRed, TeamBlue>()
            .ForEach((Entity beeEntity, int entityInQueryIndex, ref Translation translation, ref Target target, ref BeeMovement beeMovement, in DynamicBuffer<LinkedEntityGroup> group) =>
            {
                translation.Value += (math.normalize(target.TargetPosition - translation.Value) * beeMovement.CurrentVelocity * dtTime);

                float distanceSqToTarget = math.distancesq(target.TargetPosition, translation.Value);

                var random = new Random((uint)entityInQueryIndex + seed);
                beeMovement.TimeToChangeVelocity -= dtTime;
                if(beeMovement.TimeToChangeVelocity < 0.0f)
                {
                    beeMovement.CurrentVelocity = random.NextFloat(constants.MinBeeVelocity, constants.MaxBeeVelocity);

                    var newYScale = ((beeMovement.CurrentVelocity - constants.MinBeeVelocity) / (constants.MaxBeeVelocity - constants.MinBeeVelocity));
                    var nonUniformScale = new NonUniformScale { Value = new float3(1.0f, newYScale, 1.0f)};
                    ecb.SetComponent<NonUniformScale>(entityInQueryIndex, group[2].Value, nonUniformScale);

                    beeMovement.TimeToChangeVelocity = random.NextFloat(constants.MinBeeChangeVelocityTime, constants.MaxBeeChangeVelocityTime);
                }

                if (distanceSqToTarget <= 4.0f)
                {
                    if (target.TargetType == TargetType.Bee)
                    {
                        // spawn blood
                        var bloodSpawnerEntity = ecb.CreateEntity(entityInQueryIndex, particleArchetype);
                        var bloodSpawner = new ParticleSpawner
                        {
                            Prefab = prefabs.BloodPrefab,
                            Position = translation.Value,
                            Lifetime = 10.0f,
                            Speed = 8.0f,
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
                        target.TargetPosition = (HasComponent<TeamRed>(beeEntity) 
                            ? WorldUtils.GetRedHiveRandomPosition(bounds, ref random) 
                            : WorldUtils.GetBlueHiveRandomPosition(bounds, ref random));
                        ecb.RemoveComponent<Disabled>(entityInQueryIndex, group[1].Value);
                        ecb.DestroyEntity(entityInQueryIndex, target.TargetEntity);
                    }
                    else if (target.TargetType == TargetType.Hive)
                    {
                        //Dropping food.
                        target.Reset();
                        var instance = ecb.Instantiate(entityInQueryIndex, prefabs.FoodPrefab);
                        var foodInHive = new Translation {Value = translation.Value};
                        ecb.SetComponent(entityInQueryIndex, instance, foodInHive);
                        ecb.AddComponent(entityInQueryIndex, instance, new InHive{});
                        ecb.AddComponent<Disabled>(entityInQueryIndex, group[1].Value);
                    }
                    else // Wander
                    {
                        target.Reset();
                    }
                }
            }).ScheduleParallel();
        
        system.AddJobHandleForProducer(Dependency);
    }
}
