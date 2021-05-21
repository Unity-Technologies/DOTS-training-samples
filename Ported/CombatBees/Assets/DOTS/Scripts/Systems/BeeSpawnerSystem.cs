using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

[UpdateInGroup(typeof(SpawnGroup))]
[UpdateBefore(typeof(BeeUpdateGroup))]

public class BeeSpawnerSystem : SystemBase
{
    private EntityCommandBufferSystem EntityCommandBufferSystem;

    protected override void OnCreate()
    {
        EntityCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var ecb = EntityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter();
        
        var arena = GetSingletonEntity<IsArena>();
        var arenaAABB = EntityManager.GetComponentData<Bounds>(arena).Value;

        var yellowBase = GetSingletonEntity<YellowBase>();
        var yellowBaseAABB = EntityManager.GetComponentData<Bounds>(yellowBase).Value;

        var blueBase = GetSingletonEntity<BlueBase>();
        var blueBaseAABB = EntityManager.GetComponentData<Bounds>(blueBase).Value;

        var random = Utils.GetRandom();

        var spawnerEntity = GetSingletonEntity<BeeSpawner>(); 
        var beeSpawner = GetComponent<BeeSpawner>(spawnerEntity);
        var numberOfBees = beeSpawner.BeeCountFromResource;

        var explosion = GetComponent<Explosion>(spawnerEntity);

        Entities
            .WithName("SpawnBees")
            .WithAll<IsResource, OnCollision>()
            .WithNone<LifeSpan>()
            .ForEach((int entityInQueryIndex, Entity entity, in Translation translation) =>
            {
                var baseEntity = Entity.Null;
                
                if (yellowBaseAABB.Contains(translation.Value))
                {
                    baseEntity = yellowBase;
                }
                else if (blueBaseAABB.Contains(translation.Value))
                {
                    baseEntity = blueBase;
                }

                if (baseEntity != Entity.Null)
                {
                    var explosionInstance = ecb.Instantiate(entityInQueryIndex, explosion.ExplosionPrefab);
                    
                    ecb.SetComponent(entityInQueryIndex, explosionInstance, new Translation
                    {
                        Value = translation.Value
                    });

                    ecb.AddComponent(entityInQueryIndex, explosionInstance, new LifeSpan
                    {
                        Value = 4
                    });

                    for (int i = 0; i < numberOfBees; ++i)
                    {
                        var instance = ecb.Instantiate(entityInQueryIndex, beeSpawner.BeePrefab);
                        
                        ecb.SetComponent(entityInQueryIndex, instance, GetComponent<Team>(baseEntity));
                        
                        var minSpeed = random.NextFloat(0, beeSpawner.MinSpeed);
                        var maxSpeed = random.NextFloat(minSpeed, beeSpawner.MaxSpeed);

                        var randomPointOnBase = Utils.BoundedRandomPosition(arenaAABB, ref random);

                        ecb.SetComponent(entityInQueryIndex, instance, new Velocity
                        {
                            Value = math.normalize(randomPointOnBase - translation.Value) * maxSpeed
                        });

                        ecb.SetComponent(entityInQueryIndex, instance, new TargetPosition
                        {
                            Value = randomPointOnBase
                        });

                        ecb.SetComponent(entityInQueryIndex, instance, new Speed
                        {
                            MaxValue = maxSpeed,
                            MinValue = minSpeed
                        });
                        
                        ecb.SetComponent(entityInQueryIndex, instance, new Translation
                        {
                            Value = translation.Value
                        });

                        ecb.SetComponent(entityInQueryIndex, instance, new URPMaterialPropertyBaseColor
                        {
                            Value = GetComponent<URPMaterialPropertyBaseColor>(baseEntity).Value
                        });

                        var aggression = random.NextFloat(0, 1);
                        
                        ecb.SetComponent(entityInQueryIndex, instance, new Aggression
                        {
                            Value = aggression
                        });
                    }

                    ecb.AddComponent<LifeSpan>(entityInQueryIndex, entity);
                }
            }).ScheduleParallel();

        EntityCommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}
