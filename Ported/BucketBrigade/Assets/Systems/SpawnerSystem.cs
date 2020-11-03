using Unity.Entities;
using Unity.Collections;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;

public class SpawnerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        Entities
            .ForEach((Entity entity, in Spawner spawner) =>
            {
                ecb.DestroyEntity(entity);

                for (int i = 0; i < spawner.FireGridDimension; ++i)
                {
                    for (int j = 0; j < spawner.FireGridDimension; ++j)
                    {
                        var instance = ecb.Instantiate(spawner.FireCell);
                        var translation = new Translation { Value = new float3(i, -0.5f, j) };
                        ecb.SetComponent(instance, translation);
                    }
                }

                for (int i = 0; i < spawner.BucketCount; ++i)
                {
                    var instance = ecb.Instantiate(spawner.BucketPrefab);
                    var translation = new Translation { Value = new float3(UnityEngine.Random.Range(0.0f, spawner.FireGridDimension), 0.0f, UnityEngine.Random.Range(0.0f, spawner.FireGridDimension)) };
                    ecb.SetComponent(instance, translation);
                }

            }).Run();

        ecb.Playback(EntityManager);
    }
}