using System.Collections.Generic;
using Assets.Scripts;
using Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace Systems
{
    public partial class GenerationSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            var pointDisplacement = World.GetExistingSystem<PointDisplacementSystem>();

            var random = new Unity.Mathematics.Random(1234);
            NativeList<VerletPoints> points = default;
            NativeList<Link> links = default;
            NativeList<Anchor> anchors = default;

            Entities.ForEach((Entity entity, in GenerationParameters generation) =>
            {
                ecb.DestroyEntity(entity);

                GenerateCity(generation, ref ecb, random, out points, out links, out anchors);
                GenerateTornado(generation, ref ecb, random);
            }).WithoutBurst().Run();

            ecb.Playback(EntityManager);
            ecb.Dispose();

            pointDisplacement.Initialize(new NativeArray<VerletPoints>(points, Allocator.Persistent),
                new NativeArray<Link>(links, Allocator.Persistent));
            points.Dispose();
            links.Dispose();
            anchors.Dispose();
        }

        private static void GenerateTornado(in GenerationParameters spawner, ref EntityCommandBuffer ecb, Random random)
        {
            for (int i = 0; i < spawner.particleCount; ++i)
            {
                var instance = ecb.Instantiate(spawner.particlePrefab);
                var particlePosition = random.NextFloat3(
                    spawner.minParticleSpawnPosition,
                    spawner.maxParticleSpawnPosition);
                var particleScale = random.NextFloat(spawner.minParticleScale, spawner.maxParticleScale);
                var color = new float4(1.0f, 1.0f, 1.0f, 1.0f) *
                            random.NextFloat(spawner.minColorMultiplier, spawner.maxColorMultiplier);

                ecb.AddComponent(instance, new Particle {radiusMult = random.NextFloat()});
                ecb.SetComponent(instance, new Translation {Value = particlePosition});
                ecb.AddComponent(instance, new URPMaterialPropertyBaseColor {Value = color});
                ecb.AddComponent(instance, new Scale {Value = particleScale});
            }
        }


        private static void GenerateCity(in GenerationParameters generation, ref EntityCommandBuffer ecb, Random random,
            out NativeList<VerletPoints> pointsList, out NativeList<Link> linksList, out NativeList<Anchor> anchors)
        {
            linksList = new NativeList<Link>(Allocator.Temp);
            pointsList = new NativeList<VerletPoints>(Allocator.Temp);
            anchors = new NativeList<Anchor>(Allocator.Temp);

            // buildings
            for (int i = 0; i < 35; i++)
            {
                int height = random.NextInt(4, 12);
                Vector3 pos = new Vector3(random.NextFloat(-45f, 45f), 0f, random.NextFloat(-45f, 45f));
                float spacing = 2f;
                for (int j = 0; j < height; j++)
                {
                    var anchor = new Anchor {isAnchor = j == 0 ? byte.MaxValue : (byte) 0};
                    var position = new float3(pos.x + spacing, j * spacing, pos.z - spacing);
                    pointsList.Add(new VerletPoints
                    {
                        currentPosition = position,
                        oldPosition = position
                    });

                    anchors.Add(anchor);

                    position = new float3(pos.x - spacing, j * spacing, pos.z - spacing);
                    pointsList.Add(new VerletPoints
                    {
                        currentPosition = position,
                        oldPosition = position
                    });
                    anchors.Add(anchor);

                    position = new float3(pos.x + 0, j * spacing, pos.z + spacing);
                    pointsList.Add(new VerletPoints
                    {
                        currentPosition = position,
                        oldPosition = position
                    });
                    // :TODO: do we really need 3 identical copies?
                    anchors.Add(anchor);
                }
            }

            // ground details
            for (int i = 0; i < 600; i++)
            {
                var minPos = new float3(-55.0f, 0.0f, -55.0f);
                var maxPos = new float3(55.0f, 0.0f, 55.0f);
                var pos = random.NextFloat3(minPos, maxPos);
                var position = pos + random.NextFloat3(new float3(-0.2f, 0.0f, 0.1f), new float3(-0.1f, 3.0f, 0.2f));

                pointsList.Add(new VerletPoints
                {
                    currentPosition = position,
                    oldPosition = position
                });
                anchors.Add(new Anchor());

                position = pos + random.NextFloat3(new float3(0.2f, 0.0f, -0.1f), new float3(0.1f, 0.2f, -0.2f));
                pointsList.Add(new VerletPoints
                {
                    currentPosition = position,
                    oldPosition = position
                });

                anchors.Add(new Anchor {isAnchor = random.NextFloat() < 0.1f ? byte.MaxValue : (byte) 0});
            }

            int linkCount = 0;

            for (int i = 0; i < pointsList.Length; i++)
            {
                for (int j = i + 1; j < pointsList.Length; j++)
                {
                    var delta = pointsList[i].currentPosition - pointsList[j].currentPosition;
                    var linkLength = math.length(delta);

                    if (linkLength < 5f && linkLength > 0.2f)
                    {
                        var barEntity = ecb.Instantiate(generation.barPrefab);
                        var thickness = random.NextFloat(0.25f, 0.35f);
                        ecb.AddComponent(barEntity, new Bar
                        {
                            indexLink = linkCount,
                            oldDirection = new float3(0.0f, 1.0f, 0.0f),
                            thickness = thickness
                        });
                        ecb.AddComponent(barEntity,
                            new NonUniformScale {Value = new float3(thickness, thickness, linkLength)});

                        float upDot =
                            math.acos(math.abs(math.dot(new float3(0.0f, 1.0f, 0.0f), math.normalize(delta)))) /
                            math.PI;
                        var color = new float4(1.0f, 1.0f, 1.0f, 1.0f) * upDot * random.NextFloat(0.7f, 1.0f);
                        color.w = 1.0f;

                        ecb.AddComponent(barEntity, new URPMaterialPropertyBaseColor {Value = color});


                        var link = new Link {startIndex = i, endIndex = j, length = linkLength};

                        linksList.Add(link);
                        ++linkCount;
                    }
                }
            }
        }
    }
}