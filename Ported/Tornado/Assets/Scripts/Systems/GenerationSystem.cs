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
        private GenerationParameters m_cachedParameters;

        public void Reset()
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            // destroy bars
            Entities.ForEach((Entity entity, in Bar bar) =>
            {
                ecb.DestroyEntity(entity);
            }).Run();

            // destroy tornado particles
            Entities.ForEach((Entity entity, in Particle particle) =>
            {
                ecb.DestroyEntity(entity);
            }).Run();

            // Reset points, links, etc
            var pointDisplacement = World.GetExistingSystem<PointDisplacementSystem>();
            pointDisplacement.Reset();

            // add new generation entity
            var generationEntity = ecb.CreateEntity();
            ecb.AddComponent(generationEntity, m_cachedParameters);

            ecb.Playback(EntityManager);
            ecb.Dispose();
        }

        protected override void OnCreate()
        {
            RequireForUpdate(GetEntityQuery(typeof(GenerationParameters)));
        }

        protected override void OnUpdate()
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            var pointDisplacement = World.GetExistingSystem<PointDisplacementSystem>();

            var random = new Unity.Mathematics.Random(1234);
            NativeList<VerletPoints> points = default;
            NativeList<Link> links = default;

            Entities.ForEach((Entity entity, in GenerationParameters generation) =>
            {
                m_cachedParameters = generation;
                ecb.DestroyEntity(entity);

                GenerateCity(generation, ref ecb, random, out points, out links);
                GenerateTornado(generation, ref ecb, random);
            }).WithoutBurst().Run();

            ecb.Playback(EntityManager);
            ecb.Dispose();

            if (!points.IsEmpty)
            {
                var pointArray = new NativeArray<VerletPoints>(links.Length * 2, Allocator.Persistent);

                for (int i = 0; i < points.Length; i++)
                {
                    pointArray[i] = points[i];
                }

                pointDisplacement.Initialize(pointArray, new NativeArray<Link>(links, Allocator.Persistent),
                    points.Length);
                points.Dispose();
                links.Dispose();
            }
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

                ecb.SetComponent(instance, new Particle {radiusMult = random.NextFloat()});
                ecb.SetComponent(instance, new Translation {Value = particlePosition});
                ecb.SetComponent(instance, new URPMaterialPropertyBaseColor {Value = color});
                ecb.SetComponent(instance, new Scale {Value = particleScale});
            }
        }


        private static void GenerateCity(in GenerationParameters generation, ref EntityCommandBuffer ecb, Random random,
            out NativeList<VerletPoints> pointsList, out NativeList<Link> linksList)
        {
            linksList = new NativeList<Link>(Allocator.Temp);
            pointsList = new NativeList<VerletPoints>(Allocator.Temp);
           

            // buildings
            for (int i = 0; i < 35; i++)
            {
                int height = random.NextInt(4, 12);
                Vector3 pos = new Vector3(random.NextFloat(-45f, 45f), 0f, random.NextFloat(-45f, 45f));
                float spacing = 2f;
                for (int j = 0; j < height; j++)
                {
                   
                    var position = new float3(pos.x + spacing, j * spacing, pos.z - spacing);
                    var anchored =  j == 0 ? byte.MaxValue : (byte)0;
                    pointsList.Add(new VerletPoints
                    {
                        currentPosition = position,
                        oldPosition = position,
                        anchored = anchored,
                    });
                   

                    position = new float3(pos.x - spacing, j * spacing, pos.z - spacing);
                    pointsList.Add(new VerletPoints
                    {
                        currentPosition = position,
                        oldPosition = position,
                        anchored = anchored,
                    });
                   

                    position = new float3(pos.x + 0, j * spacing, pos.z + spacing);
                    pointsList.Add(new VerletPoints
                    {
                        currentPosition = position,
                        oldPosition = position,
                        anchored = anchored,
                    });                  
                 
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
                

                position = pos + random.NextFloat3(new float3(0.2f, 0.0f, -0.1f), new float3(0.1f, 0.2f, -0.2f));
                pointsList.Add(new VerletPoints
                {
                    currentPosition = position,
                    oldPosition = position,
                    anchored = random.NextFloat() < 0.1f ? byte.MaxValue : (byte)0
                });
            }

            int linkCount = 0;

            for (int i = 0; i < pointsList.Length; i++)
            {
                for (int j = i + 1; j < pointsList.Length; j++)
                {
                    var point1 = pointsList[i];
                    var point2 = pointsList[j];
                    var delta = point1.currentPosition - point2.currentPosition;
                    var linkLength = math.length(delta);
                 
                    if (linkLength < 5f && linkLength > 0.2f)
                    {
                        point1.neighborCount++;
                        point2.neighborCount++;

                        var thickness = random.NextFloat(0.25f, 0.35f);
                        var barEntity = ecb.Instantiate(generation.barPrefab);
                        ecb.SetComponent(barEntity, new Bar
                        {
                            indexLink = linkCount,
                            oldDirection = new float3(0.0f, 1.0f, 0.0f),
                            thickness = thickness
                        });
                        ecb.SetComponent(barEntity,
                            new NonUniformScale {Value = new float3(thickness, thickness, linkLength)});

                        float upDot =
                            math.acos(math.abs(math.dot(new float3(0.0f, 1.0f, 0.0f), math.normalize(delta)))) /
                            math.PI;
                      
                        //var color = new float4(1.0f, 1.0f, 1.0f, 1.0f) * upDot * random.NextFloat(0.7f, 1.0f);
                        var color = upDot >= 0.5f ? new float4(1.0f, 0.0f, 0.0f, 1.0f) : new float4(0.0f, 1.0f, 0.0f, 1.0f);

                        ecb.SetComponent(barEntity, new URPMaterialPropertyBaseColor {Value = color});

                        var link = new Link {point1Index = i, point2Index = j, length = linkLength};
                        link.materialID = (ushort)(upDot >= 0.5f ? 1 : 0);
             
                        point1.materialID = link.materialID > point1.materialID ? link.materialID : point1.materialID;
                        point2.materialID = link.materialID > point2.materialID ? link.materialID : point2.materialID;
                

                        linksList.Add(link);
                        ++linkCount;

                        pointsList[i] = point1;
                        pointsList[j] = point2;
                    }
                }
            }
        }
    }
}