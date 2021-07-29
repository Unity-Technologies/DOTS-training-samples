using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace DOTSRATS
{
    [UpdateAfter(typeof(SetupSystem))]
    public class AnimalSpawnerSystem : SystemBase
    {
        EntityCommandBufferSystem CommandBufferSystem;

        protected override void OnCreate()
        {
            CommandBufferSystem
                = World.GetExistingSystem<BeginSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var deltaTime = Time.DeltaTime;

            //var ecb = new EntityCommandBuffer(Allocator.Temp);
            var ecb = CommandBufferSystem.CreateCommandBuffer();

            int numCats = GetEntityQuery(ComponentType.ReadOnly<Cat>(), ComponentType.ReadOnly<InPlay>(), ComponentType.ReadOnly<Velocity>()).CalculateEntityCount();
            int numRats = GetEntityQuery(ComponentType.ReadOnly<Rat>(), ComponentType.ReadOnly<InPlay>(), ComponentType.ReadOnly<Velocity>()).CalculateEntityCount();

            Entities
                .WithAll<InPlay, AnimalSpawner>()
                .ForEach((Entity entity, ref AnimalSpawner animalSpawner, in Translation translation, in DirectionData direction) =>
                {
                    var isRat = HasComponent<Rat>(entity);
                    var numAnimals = isRat ? numRats : numCats;
                    if (numAnimals < animalSpawner.maxAnimals && (animalSpawner.timeToNextSpawn -= deltaTime) < 0f)
                    {
                        animalSpawner.timeToNextSpawn = animalSpawner.spawnRate;
                        var instance = ecb.Instantiate(animalSpawner.animalPrefab);
                        ecb.SetComponent(instance, translation);
                        ecb.SetComponent(instance, new Rotation { Value = quaternion.LookRotationSafe(DirectionExt.ToFloat3(direction.Value), new float3(0f, 1f, 0f)) });

                        bool spawnSmall = SpawnBigOrSmall(ref animalSpawner.random);
                        ecb.AddComponent(instance, new Velocity{Direction = direction.Value, Speed = animalSpawner.initialSpeed[spawnSmall?1:0]});
                        ecb.AddComponent(instance, new Scale { Value = spawnSmall ? 1f : 1.5f });
                        ecb.SetComponent(instance, new Scaling { targetScale = spawnSmall ? 1f : 1.5f });
                    }
                }).Schedule();

            CommandBufferSystem.AddJobHandleForProducer(Dependency);
        }

        static bool SpawnBigOrSmall(ref Random random)
            => random.NextInt(10) >= 2;
    }
}
