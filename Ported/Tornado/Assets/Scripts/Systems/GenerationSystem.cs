using System;
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
        private GenerationParameters m_cachedParameters;

        private NativeArray<bool> spawnMapBool;

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
            NativeList<int> islandPointAllocators = default;
            NativeList<int> linkStartIndices = default;

            ComputeTexture();

            Entities.ForEach((Entity entity, in GenerationParameters generation) =>
            {
                m_cachedParameters = generation;
                ecb.DestroyEntity(entity);

                GenerateCity(generation, ref ecb, ref random, out points, out links, out islandPointAllocators, out linkStartIndices, ref spawnMapBool);
                GenerateTornado(generation, ref ecb, ref random);
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
                    islandPointAllocators, linkStartIndices);
                points.Dispose();
                links.Dispose();
                linkStartIndices.Dispose();
                islandPointAllocators.Dispose();
            }
        }

        private void ComputeTexture()
        {
            var genParams = GetSingleton<GenerationParameters>();
            var gen = Resources.Load<GenerationScriptableObj>("genParams");

            if (gen.spawnMap)
            {
                var spawnMap = gen.spawnMap;

                spawnMapBool = new NativeArray<bool>(spawnMap.width * spawnMap.height, Allocator.Persistent);

                for (int x = 0; x < spawnMap.width; x++)
                {
                    for (int y = 0; y < spawnMap.height; y++)
                    {
                        spawnMapBool[x * spawnMap.height + y] = spawnMap.GetPixel(x, y).b > 0.5f;
                    }
                }

                genParams.spawnMapW = spawnMap.width;
                genParams.spawnMapH = spawnMap.height;

            }
            else spawnMapBool = new NativeArray<bool>(0, Allocator.Persistent);

            SetSingleton<GenerationParameters>(genParams);
        }

        private static void GenerateTornado(in GenerationParameters spawner, ref EntityCommandBuffer ecb, ref Random random)
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

        private static void GenerateBuilding(ref Random random, ref NativeList<VerletPoints> pointsList, in GenerationParameters genParams)
        {
            int height = random.NextInt(4, 12);
            var size = genParams.citySize / 2f;
            Vector3 pos = new Vector3(random.NextFloat(-size, size), 0f, random.NextFloat(-size, size));
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

        private static void GenerateGroundDetail(ref Random random, ref NativeList<VerletPoints> pointsList, float size)
        {            
            var minPos = new float3(-size, 0.0f, -size);
            var maxPos = new float3(size, 0.0f, size);
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

        private static void GenerateLettersAsGround(ref Random random, ref NativeList<VerletPoints> pointsList,ref NativeArray<bool> spawnMap, float size, int w, int h)
        {
            var factor = size / math.min(w, h);

            for (int x = 0; x < w; x++)
            {
                for (int z = 0; z < h; z++)
                {
                   
                    if (!spawnMap[x * h + z]) continue;

                    var pos = new float3(-(size * 0.5f) + x * factor, 0.0f, -(size * 0.5f)  + z * factor);

                    var position = pos + random.NextFloat3(new float3(0.0f, 0.0f, 0.0f), new float3(0.0f, 3.0f, 0.0f));
                    pointsList.Add(new VerletPoints
                    {
                        currentPosition = position,
                        oldPosition = position
                    });
                  
                }
            }
        }



        private static void GenerateLinks(int minPointIndex, int maxPointIndex, ref EntityCommandBuffer ecb, ref Random random, Entity barPrefab, ref NativeList<VerletPoints> pointsList, ref NativeList<Link> linksList)
        {
            for (int i = minPointIndex; i < maxPointIndex; i++)
            {
                for (int j = i + 1; j < maxPointIndex; j++)
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
                        var barEntity = ecb.Instantiate(barPrefab);

                        ecb.SetComponent(barEntity, new Bar
                        {
                            indexLink = linksList.Length,
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

                        pointsList[i] = point1;
                        pointsList[j] = point2;
                    }
                }
            }
        }

        private static void GenerateCity(in GenerationParameters generation, ref EntityCommandBuffer ecb, ref Random random,
            out NativeList<VerletPoints> pointsList, out NativeList<Link> linksList, out NativeList<int> islandPointAllocators, out NativeList<int> linkStartIndices, ref NativeArray<bool> spawnMap)
        {
            linksList = new NativeList<Link>(Allocator.Temp);
            pointsList = new NativeList<VerletPoints>(Allocator.Temp);
            islandPointAllocators = new NativeList<int>(Allocator.Temp);
            linkStartIndices = new NativeList<int>(Allocator.Temp);

            
            // buildings
            for (int i = 0; i < generation.buildings; i++)
            {
                int startingPointCount = pointsList.Length;
                GenerateBuilding(ref random, ref pointsList, generation);
                int endingPointCount = pointsList.Length;
                int startingLinkCount = linksList.Length;
                linkStartIndices.Add(startingLinkCount);
                GenerateLinks(startingPointCount, endingPointCount, ref ecb, ref random, generation.barPrefab, ref pointsList, ref linksList);

                int endingLinkCount = linksList.Length;
                int islandLinkCount = endingLinkCount - startingLinkCount;
                int fullyBrokenIslandPointCount = islandLinkCount * 2;
                int usedPointCount = endingPointCount - startingPointCount;

                int additionalPointsToAllocate = fullyBrokenIslandPointCount - usedPointCount;
                var paddingPoint = pointsList[endingPointCount - 1];
                paddingPoint.anchored = byte.MaxValue;

                for (int j = 0; j < additionalPointsToAllocate; ++j)
                {
                    pointsList.Add(paddingPoint);
                }

                islandPointAllocators.Add(usedPointCount + startingLinkCount * 2);
            }


            // ground details
            
            {    int beforeGroundDetail = pointsList.Length;
                
                for (int i = 0; i < generation.groundDetails; i++)
                {
                    GenerateGroundDetail(ref random, ref pointsList, (float)generation.citySize / 2f);
                }

                if(generation.citySize == 0 && generation.groundDetails == 0)
                    GenerateLettersAsGround(ref random, ref pointsList,ref spawnMap, (float)generation.citySize, generation.spawnMapW, generation.spawnMapH);

                int afterGroundDetail = pointsList.Length;
                int startingLinkCount = linksList.Length;
                linkStartIndices.Add(startingLinkCount);
                GenerateLinks(beforeGroundDetail, afterGroundDetail, ref ecb, ref random, generation.barPrefab,
                    ref pointsList, ref linksList);

                int endingLinkCount = linksList.Length;
                int islandLinkCount = endingLinkCount - startingLinkCount;
                int fullyBrokenIslandPointCount = islandLinkCount * 2;
                int usedPointCount = afterGroundDetail - beforeGroundDetail;

                int additionalPointsToAllocate = fullyBrokenIslandPointCount - usedPointCount;
                var paddingPoint = pointsList[afterGroundDetail - 1];
                paddingPoint.anchored = byte.MaxValue;

                for (int j = 0; j < additionalPointsToAllocate; ++j)
                {
                    pointsList.Add(paddingPoint);
                }

                islandPointAllocators.Add(usedPointCount + startingLinkCount * 2);
            }

            //GenerateLinks(0, pointsList.Length, ref ecb, random, generation.barPrefab, ref pointsList, ref linksList);
            // SortLinkIslands(ref linksList);
        }

        struct Island
        {
            public int islandIndex;
            public HashSet<int> points;
        }

        private static List<int> SortLinkIslands(ref NativeList<Link> linksList)
        {
            var islands = new Dictionary<int, Island>();
            int islandCount = 0;

            foreach (var link in linksList)
            {
                bool point1Present = islands.TryGetValue(link.point1Index, out Island islandPointsForPoint1);
                bool point2Present = islands.TryGetValue(link.point2Index, out Island islandPointsForPoint2);

                if (point1Present)
                {
                    if (point2Present)
                    {
                        // both points are in islands, but we might need to merge islands if they're currently in different islands
                        if (islandPointsForPoint1.islandIndex != islandPointsForPoint2.islandIndex)
                        {
                            var islandToExpand = islandPointsForPoint1.islandIndex < islandPointsForPoint2.islandIndex
                                ? islandPointsForPoint1
                                : islandPointsForPoint2;
                            var islandToDelete = islandPointsForPoint1.islandIndex < islandPointsForPoint2.islandIndex
                                ? islandPointsForPoint2
                                : islandPointsForPoint1;

                            islandToExpand.points.UnionWith(islandToDelete.points);

                            // update the old point2 island references to use the new merged island
                            foreach (var point in islandToDelete.points)
                            {
                                islands[point] = islandToExpand;
                            }
                        }
                    }
                    else
                    {
                        // point2 isn't in an island yet, add it to point1's island & record result under point2
                        islandPointsForPoint1.points.Add(link.point2Index);
                        islands[link.point2Index] = islandPointsForPoint1;
                    }
                }
                else
                {
                    if (point2Present)
                    {
                        // point1 isn't in an island yet, add it to point2's island & record result under point1
                        islandPointsForPoint2.points.Add(link.point1Index);
                        islands[link.point1Index] = islandPointsForPoint2;
                    }
                    else
                    {
                        // neither point is in an island yet, add a new island
                        var islandPoints = new HashSet<int> { link.point1Index, link.point2Index };
                        var islandIndex = islandCount++;
                        var island = new Island { islandIndex = islandIndex, points = islandPoints };
                        islands.Add(link.point1Index, island);
                        islands.Add(link.point2Index, island);
                    }
                }
            }

            var linkSorter = new LinkSorter(islands);

            // sort links by island index
            linksList.Sort(linkSorter);
            int lastIndex = -1;
            var islandStartIndices = new List<int>();

            for (int i = 0; i < linksList.Length; ++i)
            {
                var islandIndex = islands[linksList[i].point1Index].islandIndex;
                Debug.Assert(islandIndex == islands[linksList[i].point2Index].islandIndex);

                if (islandIndex != lastIndex)
                {
                    islandStartIndices.Add(i);
                    lastIndex = islandIndex;
                }
            }

            return islandStartIndices;
        }


        private class LinkSorter : IComparer<Link>
        {
            private Dictionary<int, Island> m_pointIslands;

            public LinkSorter(Dictionary<int, Island> pointIslands)
            {
                m_pointIslands = pointIslands;
            }
            public int Compare(Link x, Link y)
            {
                int yIslandIndex = m_pointIslands[y.point1Index].islandIndex;
                int xIslandIndex = m_pointIslands[x.point1Index].islandIndex;

                Debug.Assert(xIslandIndex == m_pointIslands[x.point2Index].islandIndex);
                Debug.Assert(yIslandIndex == m_pointIslands[y.point2Index].islandIndex);

                return xIslandIndex.CompareTo(yIslandIndex);
            }
        }
    }
}