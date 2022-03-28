using Assets.Scripts;
using Components;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Systems
{
    public partial class GenerationSystem : SystemBase
    {

       
        protected override void OnUpdate()
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

                                if(z > 0 && y > 0 && px > 0)
                                {
                                    AddIndex(px-1, y-1, z - 1, generation.cubeSize, point, actualIndex);
                                }
                                if ( y > 0 && px > 0)
                                {
                                    AddIndex(px - 1, y - 1, z, generation.cubeSize, point, actualIndex);
                                }
                                if (z > 0 && y > 0 )
                                {
                                    AddIndex(px , y - 1, z - 1, generation.cubeSize, point, actualIndex);
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