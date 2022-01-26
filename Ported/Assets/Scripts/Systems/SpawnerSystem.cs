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
        var antConfig = GetSingleton<Configuration>();

        Entities.ForEach((Entity entity, in Spawner spawner) =>
        {
            ecb.DestroyEntity(entity);

            var instance = ecb.Instantiate(spawner.ColonyPrefab);
            ecb.AddComponent<ColonyTag>(instance);

            // spawn the obstacles
            var angleStep = UnityMath.PI * 2 / antConfig.ObstaclesPerRing;
            for (int ringIdx = 1; ringIdx <= antConfig.ObstacleRingCount; ++ringIdx)
            {
                var openingStart = random.NextFloat(0, UnityMath.PI * 0.6f);
                var openingEnd = random.NextFloat(openingStart, UnityMath.PI * 0.8f);
                var openingStart2 = random.NextFloat(openingEnd, UnityMath.PI * 1.6f);
                var openingEnd2 = random.NextFloat(openingStart2, UnityMath.PI * 1.9f);
                for(int obstacleIdx = 0; obstacleIdx < antConfig.ObstaclesPerRing; ++obstacleIdx)
                {
                    var angle = obstacleIdx * angleStep;
                    if ((angle < openingStart || angle > openingEnd) && (angle < openingStart2 || angle > openingEnd2))
                    {
                        instance = ecb.Instantiate(spawner.ObstaclePrefab);
                        ecb.SetComponent(instance, new Translation { Value = new float3(UnityMath.cos(obstacleIdx * angleStep) , UnityMath.sin(obstacleIdx * angleStep), 0) * ringIdx * antConfig.ObstacleRadius });
                        ecb.AddComponent<ObstacleTag>(instance);
                    }
                }
            }

            instance = ecb.Instantiate(spawner.ResourcePrefab);
            ecb.AddComponent<ResourceTag>(instance);

            for(int i = 0; i < antConfig.AntCount; ++i)
            {
                instance = ecb.Instantiate(spawner.AntPrefab);
                ecb.AddComponent<AntTag>(instance);
                ecb.SetComponent(instance, new Rotation { Value = quaternion.RotateZ(random.NextFloat(-UnityMath.PI, UnityMath.PI)) });
            }
            
        }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}