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
            var groundTexture = new Texture2D(configuration.MapSize, configuration.MapSize, TextureFormat.RGBA32, false, false);
            groundTexture.wrapMode = TextureWrapMode.Clamp;
            groundRenderMesh.material.mainTexture = groundTexture;

            var ground = ecb.Instantiate(spawner.GroundPrefab);
            ecb.AddComponent<Ground>(ground);
            ecb.SetComponent(ground, new Ground() { Texture = groundTexture });

            var instance = ecb.Instantiate(spawner.ColonyPrefab);
            ecb.AddComponent<ColonyTag>(instance);
            var scale = EntityManager.GetComponentData<NonUniformScale>(spawner.ColonyPrefab).Value / configuration.MapSize;
            ecb.SetComponent(instance, new NonUniformScale() { Value = scale });

            // spawn grid
            instance = ecb.CreateEntity();
            var grid = new Grid2D(configuration.BucketResolution, configuration.BucketResolution);
            ecb.AddComponent(instance, grid);

            var bufferPheromone = ecb.AddBuffer<Pheromone>(instance);
            bufferPheromone.Length = (int)(grid.rowLength * grid.columnLength);
            for (int i = 0; i < bufferPheromone.Length; i++) bufferPheromone[i] = new Pheromone() { Value = 0 };

            var bufferObstacles = ecb.AddBuffer<ObstaclePositionAndRadius>(instance);
            bufferObstacles.Length = (int)(grid.rowLength * grid.columnLength);
            for (int i = 0; i < bufferObstacles.Length; i++) bufferObstacles[i] = new ObstaclePositionAndRadius();

            // spawn the obstacles
            var angleStep = UnityMath.PI * 2 / configuration.ObstaclesPerRing;
            scale = EntityManager.GetComponentData<NonUniformScale>(spawner.ObstaclePrefab).Value;
            scale.x = configuration.ObstacleRadius * 2;
            scale.y = configuration.ObstacleRadius * 2;
            scale /= (float) configuration.MapSize;
            var radiusLength = configuration.ObstacleRadius / (float) configuration.MapSize;
            for (int ringIdx = 1; ringIdx <= configuration.ObstacleRingCount; ++ringIdx)
            {
                var openingStart = random.NextFloat(0, UnityMath.PI * 0.6f);
                var openingEnd = random.NextFloat(openingStart, UnityMath.PI * 0.8f);
                var openingStart2 = random.NextFloat(openingEnd, UnityMath.PI * 1.6f);
                var openingEnd2 = random.NextFloat(openingStart2, UnityMath.PI * 1.9f);
                for (int obstacleIdx = 0; obstacleIdx < configuration.ObstaclesPerRing; ++obstacleIdx)
                {
                    var angle = obstacleIdx * angleStep;
                    if ((angle < openingStart || angle > openingEnd) && (angle < openingStart2 || angle > openingEnd2))
                    {
                        var translation = new float3(UnityMath.cos(obstacleIdx * angleStep), UnityMath.sin(obstacleIdx * angleStep), 0) * ringIdx * configuration.ObstacleRadiusStepPerRing;

                        instance = ecb.Instantiate(spawner.ObstaclePrefab);
                        ecb.SetComponent(instance, new Translation { Value = translation });
                        ecb.AddComponent<ObstacleTag>(instance);
                        ecb.SetComponent(instance, new NonUniformScale() { Value = scale });

                        // TODO: optimize this, can use some scissors
                        var xStep = 1 / (float)grid.rowLength;
                        var yStep = 1 / (float)grid.columnLength;
                        for (int i = 0; i < grid.columnLength; ++i)
                        {
                            for(int j = 0; j < grid.rowLength; ++j)
                            {
                                var x = (i + 0.5f) * xStep - 0.5f;
                                var y = (j + 0.5f) * yStep - 0.5f;
                                var minX = UnityMath.min(x, translation.x);
                                var maxX = UnityMath.max(x, translation.x);
                                var minY = UnityMath.min(y, translation.y);
                                var maxY = UnityMath.max(y, translation.y);
                                x = maxX - minX;
                                y = maxY - minY;

                                if(UnityMath.sqrt(x*x + y*y) <= radiusLength)
                                {
                                    var index = j + i * grid.rowLength;
                                    bufferObstacles[index] = bufferObstacles[index].IsValid ?
                                    new ObstaclePositionAndRadius(radiusLength,
                                    0.5f * (UnityMath.max(bufferObstacles[index].position.x, translation.x) + UnityMath.min(bufferObstacles[index].position.x, translation.x)),
                                    0.5f * (UnityMath.max(bufferObstacles[index].position.y, translation.y) + UnityMath.min(bufferObstacles[index].position.y, translation.y)))
                                    : new ObstaclePositionAndRadius(radiusLength, translation.x, translation.y);
                                }
                            }
                        }
                    }
                }
            }

            // spawn resource
            instance = ecb.Instantiate(spawner.ResourcePrefab);
            ecb.AddComponent<ResourceTag>(instance);
            ecb.SetComponent(instance, new Translation { Value = new float3(0, 10, 0) / configuration.MapSize });
            ecb.SetComponent(instance, new NonUniformScale() { Value = EntityManager.GetComponentData<NonUniformScale>(spawner.ResourcePrefab).Value / configuration.MapSize });

            var antComponents = new ComponentTypes(new ComponentType[]
            {
                typeof(AntTag),
                typeof(NonUniformScale),
                typeof(Brightness),
                typeof(Velocity),
                typeof(GeneralDirection),
                typeof(MapContainmentSteering),
                typeof(ObstacleAvoidanceSteering),
                typeof(ProximitySteering),
                typeof(PheromoneSteering),
                typeof(WanderingSteering),
                typeof(CollisionResult),
                typeof(AntMovementState),
                typeof(URPMaterialPropertyBaseColor),
            });

            // spawn ants
            for(int i = 0; i < configuration.AntCount; ++i)
            {
                instance = ecb.Instantiate(spawner.AntPrefab);
                ecb.AddComponent(instance, antComponents);
                ecb.SetComponent(instance, new Rotation { Value = quaternion.RotateZ(random.NextFloat(-UnityMath.PI, UnityMath.PI)) });
                ecb.SetComponent(instance, new Translation { Value = new float3(-4, 0, 0) / configuration.MapSize });
                ecb.SetComponent(instance, new NonUniformScale() { Value = configuration.AntSize });
                ecb.SetComponent(instance, new Brightness() { Value = random.NextFloat(0.75f, 1.25f) });
            }
        }).WithoutBurst().Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}