using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace DOTSRATS
{
    public class AnimalSpawnerSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var deltaTime = Time.DeltaTime;

            var ecb = new EntityCommandBuffer(Allocator.Temp);
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

                        bool spawnSmall = SpawnBigOrSmall(ref animalSpawner.randomSeed);
                        ecb.AddComponent(instance, new Velocity{Direction = direction.Value, Speed = animalSpawner.initialSpeed[spawnSmall?0:1]});
                    }
                }).Run();

            ecb.Playback(EntityManager);
            ecb.Dispose();
        }

        static bool SpawnBigOrSmall(ref uint randomSeed)
        {
            randomSeed = Random.CreateFromIndex(randomSeed).NextUInt();
            return (randomSeed % 10) >= 2;
        }
    }
}
