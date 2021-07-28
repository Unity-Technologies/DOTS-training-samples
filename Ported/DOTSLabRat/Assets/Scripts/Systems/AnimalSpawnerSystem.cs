using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

using Random = UnityEngine.Random;
namespace DOTSRATS
{
    public class AnimalSpawnerSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            int numCats = GetEntityQuery(ComponentType.ReadOnly<Cat>(), ComponentType.ReadOnly<InPlay>(), ComponentType.ReadOnly<Velocity>()).CalculateEntityCount();
            int numRats = GetEntityQuery(ComponentType.ReadOnly<Rat>(), ComponentType.ReadOnly<InPlay>(), ComponentType.ReadOnly<Velocity>()).CalculateEntityCount();

            Entities
                .WithAll<InPlay, AnimalSpawner, Cat>()
                .ForEach((Entity entity, in AnimalSpawner animalSpawner, in Translation translation, in DirectionData direction) =>
                {
                    if (numCats < animalSpawner.maxAnimals)
                    {
                        if (Random.Range(0f, 1f) < animalSpawner.spawnRate)
                        {
                            var instance = ecb.Instantiate(animalSpawner.animalPrefab);
                            ecb.SetComponent(instance, translation);
                            ecb.AddComponent(instance, new Velocity{Direction = direction.Value, Speed = 1f});
                        }
                    }
                }).Run();

            Entities
                .WithAll<InPlay, AnimalSpawner, Rat>()
                .ForEach((Entity entity, in AnimalSpawner animalSpawner, in Translation translation, in DirectionData direction) =>
                {
                    if (numRats < animalSpawner.maxAnimals)
                    {
                        if (Random.Range(0f, 1f) < animalSpawner.spawnRate)
                        {
                            var instance = ecb.Instantiate(animalSpawner.animalPrefab);
                            ecb.SetComponent(instance, translation);
                            ecb.AddComponent(instance, new Velocity { Direction = direction.Value, Speed = 1f });
                        }
                    }
                }).Run();

            ecb.Playback(EntityManager);
            ecb.Dispose();
        }
    }
}
