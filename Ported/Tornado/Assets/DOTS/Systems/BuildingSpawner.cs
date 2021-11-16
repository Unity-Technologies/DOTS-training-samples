using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace Dots
{
    public partial class BuildingSpawner : SystemBase
    {
        
        protected override void OnUpdate()
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            // TODO : add better approximations of point count based on building counts 
            var random = new Random(1234);
            var pointEntityList = new NativeArray<Entity>(20000, Allocator.Temp);
            var pointPosList = new NativeArray<float3>(20000, Allocator.Temp);
            var pointNeighborList = new NativeArray<int>(20000, Allocator.Temp);
            var pointGroupIdList = new NativeArray<int>(20000, Allocator.Temp);
            
            int pointCount = 0;
            int beamCount = 0;
            bool useBeamGroup = true;
            
            Entities
                .ForEach((Entity entity, in BuildingSpawnerData spawner) =>
                {
                    // Destroying the current entity is a classic ECS pattern,
                    // when something should only be processed once then forgotten.
                    ecb.DestroyEntity(entity);
                    
                    
                    void PointEntityList(float3 pointPosition, int groupId, bool allowFixedAnchor)
                    {
                        var pointEntity = ecb.CreateEntity();
                        ecb.AddComponent(pointEntity, new Anchor());
                        ecb.AddComponent(pointEntity, new Point
                        {
                            value = pointPosition,
                            old = pointPosition
                        });
                        if (pointPosition.y == 0)
                        {
                            ecb.AddComponent(pointEntity, new FixedAnchor());
                        }
                        pointEntityList[pointCount] = pointEntity;
                        pointPosList[pointCount] = pointPosition;
                        pointNeighborList[pointCount] = 0;
                        pointGroupIdList[pointCount] = groupId;
                        ++pointCount;
                    }

                    useBeamGroup = spawner.UseBeamGroups;
                    
                    float spacing = 2f; // spawner config ??
                    
                    // buildings
                    for (int i = 0; i < spawner.BuildingCount; i++)
                    {
                        int height = random.NextInt(4, 12);
                        float3 pos = new float3(random.NextFloat(-45.0f, 45.0f), 0f, random.NextFloat(-45.0f, 45.0f));
                        for (int j = 0; j < height; j++)
                        {
                            int groupId = spawner.UseBeamGroups ? i : 0;
                            PointEntityList(new float3(pos.x + spacing, j * spacing, pos.z - spacing), groupId, true);
                            PointEntityList(new float3(pos.x - spacing, j * spacing, pos.z - spacing), groupId, true);
                            PointEntityList(new float3(pos.x - 0f, j * spacing, pos.z + spacing), groupId, true);
                        }
                    }

                    // ground details
                    int groundDetailGroupId = spawner.UseBeamGroups ? spawner.BuildingCount : 0;
                    for (int i = 0; i < 600; i++)
                    {
                        float3 pos = new float3(random.NextFloat(-55f, 55f), 0f, random.NextFloat(-55f, 55f));
                        PointEntityList(new float3(
                            pos.x + random.NextFloat(-.2f, -.1f),
                            pos.y + random.NextFloat(0f, 3f),
                            pos.z + random.NextFloat(.1f, .2f)), groundDetailGroupId, false);
                        PointEntityList(new float3(
                            pos.x + random.NextFloat(.2f, .1f),
                            pos.y + random.NextFloat(0f, .2f),
                            pos.z + random.NextFloat(-.1f, -.2f)), groundDetailGroupId, true);
                    }

                    float4 white = new float4(1f, 1f, 1f, 1f);
                    for (int i = 0; i < pointCount; i++)
                    {
                        for (int j = i + 1; j < pointCount; j++)
                        {
                            float3 p1 = pointPosList[i];
                            float3 p2 = pointPosList[j];
                            bool areBothPointInSameGroup = (pointGroupIdList[i] == pointGroupIdList[j] ? true: false);
                            
                            float3 pointDelta = p2 - p1;
                            float length = math.length(pointDelta);
                            if (areBothPointInSameGroup && length < 5f && length > .2f)
                            {
                                var beamEntity = ecb.Instantiate(spawner.BeamPrefab);
                                ecb.AddComponent(beamEntity, new Beam {
                                    p1 = pointEntityList[i],
                                    p2 = pointEntityList[j],
                                    oldDelta = float3.zero
                                });

                                float3 up = new float3(0f, 1f, 0f);
                                float3 pointDeltaNorm = math.normalize(pointDelta);
                                float upDot = math.acos(math.abs(math.dot(up, pointDeltaNorm))) / Mathf.PI;
                                float4 color =  white * upDot * random.NextFloat(.7f, 1f);
                                ecb.AddComponent(beamEntity, new URPMaterialPropertyBaseColor { Value = color });
                                
                                float3 pos = new float3(p1.x + p2.x, p1.y + p2.y, p1.z + p2.z) * .5f;
                                Quaternion rot = Quaternion.LookRotation(pointDelta);
                                float thickness = random.NextFloat(0.25f, 0.35f);
                                float3 scale = new float3(thickness, thickness, length);
                                
                                ecb.SetComponent(beamEntity, new Translation { Value = pos } );
                                ecb.SetComponent(beamEntity, new Rotation { Value = rot } );
                                ecb.AddComponent(beamEntity, new NonUniformScale { Value = scale } );
                                ecb.AddComponent(beamEntity, new Length { value = length });
                                
                                pointNeighborList[i]++;
                                pointNeighborList[j]++;

                                beamCount++;
                            }
                        }
                    }

                    // Update all the Neighbor data for each points
                    for (int i = 0; i < pointCount; i++)
                    {
                        ecb.SetComponent(pointEntityList[i], new Anchor { NeighborCount = pointNeighborList[i] });
                    }
                }).Run();

            if (useBeamGroup)
            {
                // Since AddSharedComponent is not allowed in burst-able code, we'll add them now...
                for (int i = 0; i < pointCount; i++)
                {
                    ecb.AddSharedComponent(pointEntityList[i], new BeamGroup { groupId = pointGroupIdList[i] });
                }
            }

            Debug.Log(pointCount + " points, room for " + pointPosList.Length + " (" + beamCount + " beams)");
            pointEntityList.Dispose();
            pointPosList.Dispose();
            pointNeighborList.Dispose();
            pointGroupIdList.Dispose();
        
            ecb.Playback(EntityManager);
            ecb.Dispose();
        }
    }
}