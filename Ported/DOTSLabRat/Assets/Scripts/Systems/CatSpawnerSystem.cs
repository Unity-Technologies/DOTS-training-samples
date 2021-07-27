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
            EntityQuery cats = GetEntityQuery(ComponentType.ReadOnly<Cat>());

            Entities
                .ForEach((Entity entity, in CatSpawner catSpawner) =>
                {
                    if (cats.CalculateEntityCount() < catSpawner.maxCats)
                    {
                        if (Random.Range(0, 1) < catSpawner.spawnRate)
                        {
                            var instance = ecb.Instantiate(catSpawner.catPrefab);
                            var translation = new Translation { Value = new float3(Random.Range(0f, 10f), 0, Random.Range(0f, 10f)) };
                            ecb.SetComponent(instance, translation);
                        }
                    }
                }).Run();

            ecb.Playback(EntityManager);
            ecb.Dispose();
        }
    }
}