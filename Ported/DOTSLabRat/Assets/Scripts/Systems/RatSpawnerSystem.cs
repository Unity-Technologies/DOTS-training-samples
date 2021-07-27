using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

using Random = UnityEngine.Random;
namespace DOTSRATS
{
    public class RatSpawnerSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            int numRats = GetEntityQuery(ComponentType.ReadOnly<Rat>()).CalculateEntityCount();
            Entities
                .ForEach((Entity entity, in RatSpawner ratSpawner) =>
                {
                    if (numRats < ratSpawner.maxRats)
                    {
                        if (Random.Range(0, 1) < ratSpawner.spawnRate)
                        {
                            var instance = ecb.Instantiate(ratSpawner.ratPrefab);
                            //var translation = new Translation { Value = new float3(Random.Range(0f, 10f), 0, Random.Range(0f, 10f)) };
                            Translation translation = new Translation { Value = (Random.Range(0f, 1f) < 0.5 ? ratSpawner.spawnPointOne : ratSpawner.spawnPointTwo) };
                            ecb.SetComponent(instance, translation);
                        }
                    }
                }).Run();

            ecb.Playback(EntityManager);
            ecb.Dispose();
        }
    }
}