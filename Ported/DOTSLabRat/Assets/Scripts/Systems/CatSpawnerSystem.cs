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
                .ForEach((Entity entity, in CatSpawner catSpawner) =>
                {
                    if (numCats < catSpawner.maxCats)
                    {
                        if (Random.Range(0, 1) < catSpawner.spawnRate)
                        {
                            var instance = ecb.Instantiate(catSpawner.catPrefab);
                            //var translation = new Translation { Value = new float3(Random.Range(0f, 10f), 0, Random.Range(0f, 10f)) };
                            Translation translation = new Translation { Value = (Random.Range(0f, 1f) < 0.5 ? catSpawner.spawnPointOne : catSpawner.spawnPointTwo) };
                            ecb.SetComponent(instance, translation);
                        }
                    }
                }).Run();

            ecb.Playback(EntityManager);
            ecb.Dispose();
        }
    }
}