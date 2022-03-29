using Assets.Scripts;
using Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

namespace Systems
{
    public partial class GenerationSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Temp_CubeGen();

            // run once on spawner entity & delete it (maybe?)
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            // The PRNG (pseudorandom number generator) from Unity.Mathematics is a struct
            // and can be used in jobs. For simplicity and debuggability in development,
            // we'll initialize it with a constant. (In release, we'd want a seed that
            // randomly varies, such as the time from the user's system clock.)
            var random = new Unity.Mathematics.Random(1234);

            Entities
                .ForEach((Entity entity, in GenerationParameters spawner) =>
                {
                    ecb.DestroyEntity(entity);

                    for (int i = 0; i < spawner.particleCount; ++i)
                    {
                        var instance = ecb.Instantiate(spawner.particlePrefab);
                        var particlePosition = random.NextFloat3(
                            spawner.minParticleSpawnPosition,
                            spawner.maxParticleSpawnPosition);
                        var particleScale = random.NextFloat(spawner.minParticleScale, spawner.maxParticleScale);
                        var color = new float4(1.0f, 1.0f, 1.0f, 1.0f) *
                                    random.NextFloat(spawner.minColorMultiplier, spawner.maxColorMultiplier);

                        ecb.AddComponent(instance, new Particle { radiusMult = random.NextFloat()});
                        ecb.SetComponent(instance, new Translation { Value = particlePosition });
                        ecb.AddComponent(instance, new URPMaterialPropertyBaseColor {Value = color});
                        ecb.AddComponent(instance, new Scale { Value = particleScale });
                    }
                }).WithoutBurst().Run();

            ecb.Playback(EntityManager);
            ecb.Dispose();
        }

        protected void Temp_CubeGen()
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            var pointDisplacement = World.GetExistingSystem<PointDisplacementSystem>();

            NativeArray<VerletPoints> points = default;
            NativeList<Link> links = default;

            Entities.ForEach((Entity entity, in GenerationParameters generation) =>
            {
                points = new NativeArray<VerletPoints>(generation.cubeSize * generation.cubeSize * generation.cubeSize, Allocator.Temp);
                links = new NativeList<Link>(Allocator.Temp);

                ecb.DestroyEntity(entity);

                //points pass
                for (int x = 0; x < generation.cubeSize; ++x)
                {
                    for (int y = 0; y < generation.cubeSize; y++)
                    {
                        for (int z = 0; z < generation.cubeSize; z++)
                        {
                            var pos = new float3(x * 2, 2 + y * 2, z * 2);
                            var actualIndex = utils.ToIndex(x, y, z, generation.cubeSize);

                            var point = new VerletPoints()
                            {
                                oldPosition = pos,
                                currentPosition = pos,
                            };
                            points[actualIndex] = point;

                            var instance = ecb.Instantiate(generation.barPrefab);

                            var translation = new Translation { Value = pos };
                            ecb.SetComponent(instance, translation);
                            ecb.SetComponent(instance, new Bar()
                            {
                                thickness = 0.7f,
                                indexPoint = actualIndex
                            });
                        }
                    }
                }

                //links pass
                for (int px = 0; px < generation.cubeSize; ++px)
                {
                    for (int y = 0; y < generation.cubeSize; y++)
                    {
                        for (int z = 0; z < generation.cubeSize; z++)
                        {
                            int actualIndex = utils.ToIndex(px, y, z, generation.cubeSize);
                            var point = points[actualIndex];
                            int otherIndex;
                            VerletPoints other;

                            void AddIndex(int x, int y, int z, int s, VerletPoints actual, int actualIndex)
                            {
                                otherIndex = utils.ToIndex(x, y, z, s);
                                other = points[otherIndex];
                                links.Add(new Link()
                                {
                                    startIndex = actualIndex,
                                    endIndex = otherIndex,
                                    length = utils.Magnitude(actual.currentPosition - other.currentPosition)
                                });

                            }

                            if (z > 0 && y > 0 && px > 0)
                            {
                                AddIndex(px - 1, y - 1, z - 1, generation.cubeSize, point, actualIndex);
                            }
                            if (y > 0 && px > 0)
                            {
                                AddIndex(px - 1, y - 1, z, generation.cubeSize, point, actualIndex);
                            }
                            if (z > 0 && y > 0)
                            {
                                AddIndex(px, y - 1, z - 1, generation.cubeSize, point, actualIndex);
                            }
                            if (px > 0)
                            {
                                AddIndex(px - 1, y, z, generation.cubeSize, point, actualIndex);
                            }
                            if (y > 0)
                            {
                                AddIndex(px, y - 1, z, generation.cubeSize, point, actualIndex);
                            }
                            if (z > 0)
                            {
                                AddIndex(px, y, z - 1, generation.cubeSize, point, actualIndex);
                            }
                        }
                    }
                }
            }).Run();

            ecb.Playback(EntityManager);
            ecb.Dispose();

            pointDisplacement.Initialize(new NativeArray<VerletPoints>(points, Allocator.Persistent), new NativeArray<Link>(links, Allocator.Persistent));
            points.Dispose();
            links.Dispose();
        }
    }
}