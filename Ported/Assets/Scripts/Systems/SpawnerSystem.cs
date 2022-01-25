using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityMath = Unity.Mathematics.math;

public partial class SpawnerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        // The PRNG (pseudorandom number generator) from Unity.Mathematics is a struct
        // and can be used in jobs. For simplicity and debuggability in development,
        // we'll initialize it with a constant. (In release, we'd want a seed that
        // randomly varies, such as the time from the user's system clock.)
        var random = new Random(1234);

        Entities.ForEach((Entity entity, in Spawner spawner) =>
        {
            ecb.DestroyEntity(entity);

            var instance = ecb.Instantiate(spawner.ColonyPrefab);
            ecb.AddComponent<ColonyTag>(instance);

            // spawn the obstacles
            var ringDistance = 3;
            var obstaclesPerRing = 100;
            var angleStep = UnityMath.PI * 2 / obstaclesPerRing;
            for (int ringIdx = 1; ringIdx < 4; ++ringIdx)
            {
                var openingStart = random.NextFloat(0, UnityMath.PI * 0.6f);
                var openingEnd = random.NextFloat(openingStart, UnityMath.PI * 0.8f);
                var openingStart2 = random.NextFloat(openingEnd, UnityMath.PI * 1.6f);
                var openingEnd2 = random.NextFloat(openingStart2, UnityMath.PI * 1.9f);
                for(int obstacleIdx = 0; obstacleIdx < obstaclesPerRing; ++obstacleIdx)
                {
                    var angle = obstacleIdx * angleStep;
                    if ((angle < openingStart || angle > openingEnd) && (angle < openingStart2 || angle > openingEnd2))
                    {
                        instance = ecb.Instantiate(spawner.ObstaclePrefab);
                        ecb.SetComponent(instance, new Translation { Value = new float3(UnityMath.cos(obstacleIdx * angleStep) , UnityMath.sin(obstacleIdx * angleStep), 0) * ringIdx * ringDistance });
                        ecb.AddComponent<ObstacleTag>(instance);
                    }
                }
            }

            instance = ecb.Instantiate(spawner.ResourcePrefab);
            ecb.AddComponent<ResourceTag>(instance);

            instance = ecb.Instantiate(spawner.AntPrefab);
            var translation = new Translation { Value = new float3(-4, 0, 0) };
            ecb.SetComponent(instance, translation);
            
        }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}