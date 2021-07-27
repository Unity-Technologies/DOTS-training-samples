using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

using Random = UnityEngine.Random;
namespace DOTSRATS
{
    public class CatSpawnerSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            int numCats = GetEntityQuery(ComponentType.ReadOnly<Cat>()).CalculateEntityCount();

            Entities
                .WithAll<InPlay, CatSpawner>()
                .ForEach((Entity entity, in CatSpawner catSpawner, in Translation translation) =>
                {
                    if (numCats < catSpawner.maxCats)
                    {
                        if (Random.Range(0, 1) < catSpawner.spawnRate)
                        {
                            var instance = ecb.Instantiate(catSpawner.catPrefab);
                            ecb.SetComponent(instance, translation);
                            ecb.AddComponent(instance, new Velocity{Direction = Direction.North, Speed = 1f});
                        }
                    }
                }).Run();

            ecb.Playback(EntityManager);
            ecb.Dispose();
        }
    }
}