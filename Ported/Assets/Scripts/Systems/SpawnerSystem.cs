using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;
using UnityMath = Unity.Mathematics.math;

public partial class SpawnerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        var configuration = GetSingleton<Configuration>();

        // The PRNG (pseudorandom number generator) from Unity.Mathematics is a struct
        // and can be used in jobs. For simplicity and debuggability in development,
        // we'll initialize it with a constant. (In release, we'd want a seed that
        // randomly varies, such as the time from the user's system clock.)
        var random = new Random(1234);

        Entities.ForEach((Entity entity, in Spawner spawner) =>
        {
            ecb.DestroyEntity(entity);

            var groundRenderMesh = EntityManager.GetSharedComponentData<RenderMesh>(spawner.GroundPrefab);
            var groundTexture = new Texture2D(configuration.MapSize, configuration.MapSize);
            groundTexture.wrapMode = TextureWrapMode.Mirror;
            groundRenderMesh.material.mainTexture = groundTexture;

            var ground = ecb.Instantiate(spawner.GroundPrefab);
            ecb.AddComponent<Ground>(ground);
            ecb.SetComponent(ground, new Ground() { Texture = groundTexture });

            var instance = ecb.Instantiate(spawner.ColonyPrefab);
            ecb.AddComponent<ColonyTag>(instance);
            var scale = EntityManager.GetComponentData<NonUniformScale>(spawner.ColonyPrefab).Value / configuration.MapSize;
            ecb.SetComponent(instance, new NonUniformScale() { Value = scale });

            // spawn the obstacles
            var ringDistance = .2f;
            var obstaclesPerRing = 100;
            var angleStep = UnityMath.PI * 2 / obstaclesPerRing;
            scale = EntityManager.GetComponentData<NonUniformScale>(spawner.ObstaclePrefab).Value;
            scale.x = configuration.ObstacleRadius * 2;
            scale.y = configuration.ObstacleRadius * 2;
            scale /= configuration.MapSize;
            for (int ringIdx = 1; ringIdx < 4; ++ringIdx)
            {
                var openingStart = random.NextFloat(0, UnityMath.PI * 0.6f);
                var openingEnd = random.NextFloat(openingStart, UnityMath.PI * 0.8f);
                var openingStart2 = random.NextFloat(openingEnd, UnityMath.PI * 1.6f);
                var openingEnd2 = random.NextFloat(openingStart2, UnityMath.PI * 1.9f);
                for (int obstacleIdx = 0; obstacleIdx < obstaclesPerRing; ++obstacleIdx)
                {
                    var angle = obstacleIdx * angleStep;
                    if ((angle < openingStart || angle > openingEnd) && (angle < openingStart2 || angle > openingEnd2))
                    {
                        instance = ecb.Instantiate(spawner.ObstaclePrefab);
                        ecb.SetComponent(instance, new Translation { Value = new float3(UnityMath.cos(obstacleIdx * angleStep), UnityMath.sin(obstacleIdx * angleStep), 0) * ringIdx * ringDistance });
                        ecb.AddComponent<ObstacleTag>(instance);
                        ecb.SetComponent(instance, new NonUniformScale() { Value = scale });
                    }
                }
            }

            instance = ecb.Instantiate(spawner.ResourcePrefab);
            ecb.AddComponent<ResourceTag>(instance);
            ecb.SetComponent(instance, new Translation { Value = new float3(0, 10, 0) / configuration.MapSize });
            scale = EntityManager.GetComponentData<NonUniformScale>(spawner.ResourcePrefab).Value / configuration.MapSize;
            ecb.SetComponent(instance, new NonUniformScale() { Value = scale });

            for(int i = 0; i < 1000; ++i)
            {
                instance = ecb.Instantiate(spawner.AntPrefab);
                ecb.AddComponent<AntTag>(instance);
                ecb.SetComponent(instance, new Rotation { Value = quaternion.RotateZ(random.NextFloat(-UnityMath.PI, UnityMath.PI)) });
                ecb.SetComponent(instance, new Translation { Value = new float3(-4, 0, 0) / configuration.MapSize });
                ecb.AddComponent<NonUniformScale>(instance);
                ecb.SetComponent(instance, new NonUniformScale() { Value = configuration.AntSize });
            }
        }).WithoutBurst().Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}