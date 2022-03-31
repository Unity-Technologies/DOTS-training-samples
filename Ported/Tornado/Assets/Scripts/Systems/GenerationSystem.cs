using System;
using System.Collections.Generic;
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
            NativeList<int> islandPointAllocators = default;
            NativeList<int> linkStartIndices = default;

            Entities.ForEach((Entity entity, in GenerationParameters generation) =>
            {
                m_cachedParameters = generation;
                ecb.DestroyEntity(entity);

                GenerateCity(generation, ref ecb, ref random, out points, out links, out islandPointAllocators, out linkStartIndices);
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

        private static void GenerateBuilding(ref Random random, ref NativeList<VerletPoints> pointsList)
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

        private static void GenerateGroundDetail(ref Random random, ref NativeList<VerletPoints> pointsList)
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

        private static void GenerateLinks(int minPointIndex, int maxPointIndex, ref EntityCommandBuffer ecb, ref Random random, Entity barPrefab, ref NativeList<VerletPoints> pointsList, ref NativeList<Link> linksList, int linkOffset = 0)
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
                            indexLink = linksList.Length + linkOffset,
                            oldDirection = new float3(0.0f, 1.0f, 0.0f),
                            thickness = thickness
                        });
                        ecb.SetComponent(barEntity,
                            new NonUniformScale {Value = new float3(thickness, thickness, linkLength)});

                        float upDot =
                            math.acos(math.abs(math.dot(new float3(0.0f, 1.0f, 0.0f), math.normalize(delta)))) /
                            math.PI;
                      
                        //var color = new float4(1.0f, 1.0f, 1.0f, 1.0f) * upDot * random.NextFloat(0.7f, 1.0f);
                        //var color = upDot >= 0.5f ? new float4(1.0f, 0.0f, 0.0f, 1.0f) : new float4(0.0f, 1.0f, 0.0f, 1.0f);
                        var color = math.lerp(new float4(1.0f, 0.0f, 0.0f, 1.0f), new float4(0.0f, 1.0f, 0.0f, 1.0f),
                            upDot * 2.0f);
                            //upDot >= 0.5f ? new float4(1.0f, 0.0f, 0.0f, 1.0f) : new float4(0.0f, 1.0f, 0.0f, 1.0f);

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
            out NativeList<VerletPoints> pointsList, out NativeList<Link> linksList, out NativeList<int> islandPointAllocators, out NativeList<int> linkStartIndices)
        {
            linksList = new NativeList<Link>(Allocator.Temp);
            pointsList = new NativeList<VerletPoints>(Allocator.Temp);
            islandPointAllocators = new NativeList<int>(Allocator.Temp);
            linkStartIndices = new NativeList<int>(Allocator.Temp);

            // buildings
            for (int i = 0; i < 35; i++)
            {
                int startingPointCount = pointsList.Length;
                GenerateBuilding(ref random, ref pointsList);
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
            {
                int beforeGroundDetail = pointsList.Length;

                var groundDetailPoints = new NativeList<VerletPoints>(Allocator.Temp);
                var groundDetailLinks = new NativeList<Link>(Allocator.Temp);

                for (int i = 0; i < 600; i++)
                {
                    GenerateGroundDetail(ref random, ref groundDetailPoints);
                }

                int startingLinkCount = linksList.Length;

                GenerateLinks(0, groundDetailPoints.Length, ref ecb, ref random, generation.barPrefab,
                    ref groundDetailPoints, ref groundDetailLinks, startingLinkCount);

                NativeList<int> groundDetailPointAllocators;

                var linkIslandStartIndices = SeparateLinkIslands(ref groundDetailPoints, ref groundDetailLinks, out groundDetailPointAllocators);

                pointsList.AddRange(groundDetailPoints);

                for (int i = 0; i < groundDetailLinks.Length; ++i)
                {
                    var link = groundDetailLinks[i];
                    link.point1Index += beforeGroundDetail;
                    link.point2Index += beforeGroundDetail;
                    linksList.Add(link);
                }

                for (int i = 0; i < linkIslandStartIndices.Count; ++i)
                {
                    linkStartIndices.Add(linkIslandStartIndices[i] + startingLinkCount);
                }

                for (int i = 1; i < groundDetailPointAllocators.Length; ++i)
                {
                    islandPointAllocators.Add(groundDetailPointAllocators[i] + startingLinkCount * 2);
                }
            }

            //GenerateLinks(0, pointsList.Length, ref ecb, random, generation.barPrefab, ref pointsList, ref linksList);
            // SortLinkIslands(ref linksList);
        }

        struct Island
        {
            public int islandIndex;
            public HashSet<int> points;
        }

        private static List<int> SeparateLinkIslands(ref NativeList<VerletPoints> pointsList, ref NativeList<Link> linksList, out NativeList<int> pointAllocators)
        {
            var islands = DetectLinkIslands(linksList);
            var linkSorter = new LinkSorter(islands);

            // sort links by island index
            linksList.Sort(linkSorter);
            int lastIndex = -1;
            int lastIslandStartIndex = 0;
            var islandStartIndices = new List<int>();
            var oldPointToNewPointMapping = new Dictionary<int, int>();
            var reindexedPointList = new NativeArray<VerletPoints>(linksList.Length * 2, Allocator.Temp);
            pointAllocators = new NativeList<int>(Allocator.Temp);
            int indexAllocator = 0;

            for (int i = 0; i < linksList.Length; ++i)
            {
                var islandIndex = islands[linksList[i].point1Index].islandIndex;
                Debug.Assert(islandIndex == islands[linksList[i].point2Index].islandIndex);

                if (islandIndex != lastIndex)
                {
                    islandStartIndices.Add(i);
                    pointAllocators.Add(indexAllocator);

                    int newIndexAllocatorStart = i * 2;

                    // pad out point gaps
                    for (int j = indexAllocator; j < newIndexAllocatorStart; ++j)
                    {
                        var paddingPoint = reindexedPointList[indexAllocator - 1];
                        paddingPoint.anchored = byte.MaxValue;
                        reindexedPointList[j] = paddingPoint;
                    }

                    indexAllocator = newIndexAllocatorStart;
                    lastIndex = islandIndex;
                }

                // while we're here, re-index the links so that the starts & ends are next to each other in memory
                var link = linksList[i];

                if (!oldPointToNewPointMapping.TryGetValue(linksList[i].point1Index, out int newPoint1Index))
                {
                    newPoint1Index = indexAllocator++;
                    oldPointToNewPointMapping.Add(linksList[i].point1Index, newPoint1Index);
                    reindexedPointList[newPoint1Index] = pointsList[linksList[i].point1Index];
                }

                link.point1Index = newPoint1Index;

                if (!oldPointToNewPointMapping.TryGetValue(linksList[i].point2Index, out int newPoint2Index))
                {
                    newPoint2Index = indexAllocator++;
                    oldPointToNewPointMapping.Add(linksList[i].point2Index, newPoint2Index);
                    reindexedPointList[newPoint2Index] = pointsList[linksList[i].point2Index];
                }

                link.point2Index = newPoint2Index;
                linksList[i] = link;
            }

            pointAllocators.Add(indexAllocator);
            pointsList.Clear();
            pointsList.AddRange(reindexedPointList);
            reindexedPointList.Dispose();

            return islandStartIndices;
        }

        private static Dictionary<int, Island> DetectLinkIslands(in NativeList<Link> linksList)
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

            return islands;
        }

        private static List<int> SortLinkIslands(ref NativeList<Link> linksList)
        {
            var islands = DetectLinkIslands(linksList);

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