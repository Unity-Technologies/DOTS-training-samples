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
        //private EntityArchetype m_BuildingArchetype;
        
        protected override void OnCreate()
        {
            //m_BuildingArchetype = EntityManager.CreateArchetype(typeof(Building));
        }
        
        protected override void OnUpdate()
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            // TODO : add better approximations of point count based on building counts 
            var random = new Random(1234);
            
            var buildingSpawnerEntity = GetSingletonEntity<BuildingSpawnerData>();
            var buildingSpawner = GetComponent<BuildingSpawnerData>(buildingSpawnerEntity);

            // ground details
            var groundSectionAreas = new NativeArray<float4>(9, Allocator.TempJob);
            for (int i = 0; i < 3; ++i)
            {
                for (int j = 0; j < 3; ++j)
                {
                    groundSectionAreas[i * 3 + j] = new float4(
                        i * 40f - 60f,
                        (i + 1) * 40f - 60f,
                        j * 40f - 60f,
                        (j + 1) * 40f - 60f);
                }
            }

            var buildingCount = buildingSpawner.BuildingCount + 9; // we have 9 ground areas
            var beamCountApprox = buildingCount * 12 * 3 * 16; // Max 12 stairs, 3 point by stair, ~16 beam by stair
            
            //var buildingsIndices = new NativeArray<int4>(buildingCount, Allocator.Persistent);
            var anchorPoints = new NativeArray<AnchorPoint>(beamCountApprox * 2, Allocator.Persistent);
            var beams = new NativeArray<Beam>(beamCountApprox, Allocator.Persistent);

            var anchorPointRealTotal = 0;
            var anchorPointTotal = 0;
            var beamTotal = 0;

            buildingCount = 0;
            Entities
                .WithName("BuildingSpawner")
                .WithReadOnly(groundSectionAreas)
                .WithDisposeOnCompletion(groundSectionAreas)
                .ForEach((Entity entity, in BuildingSpawnerData spawner) =>
                {
                    // Destroying the current entity is a classic ECS pattern,
                    // when something should only be processed once then forgotten.
                    ecb.DestroyEntity(entity);

                    float spacing = 2f; // spawner config ??

                    int generateBeamsForBuilding(int anchorStart, int anchorCount, int beamStart, BuildingSpawnerData spawner)
                    {
                        float4 white = new float4(1f, 1f, 1f, 1f);
                        int beamCount = 0;
                        for (int i = 0; i < anchorCount; i++)
                        {
                            int indexP1 = anchorStart + i;
                            AnchorPoint p1 = anchorPoints[indexP1];
                            for (int j = i + 1; j < anchorCount; j++)
                            {
                                int indexP2 = anchorStart + j;
                                AnchorPoint p2 = anchorPoints[indexP2];

                                float3 pointDelta = p2.position - p1.position;
                                float length = math.length(pointDelta);
                                if (length < 5f && length > .2f)
                                {
                                    int beamIndex = beamStart + beamCount;
                                    Beam beam = new Beam();
                                    beam.points = new int2(i,j);
                                    beam.oldDelta = new float3(0f, 0f, 0f);

                                    var beamEntity = ecb.Instantiate(spawner.BeamPrefab);
                                    float3 up = new float3(0f, 1f, 0f);
                                    float3 pointDeltaNorm = math.normalize(pointDelta);
                                    float upDot = math.acos(math.abs(math.dot(up, pointDeltaNorm))) / Mathf.PI;
                                    float4 color = white * upDot * random.NextFloat(.7f, 1f);
                                    ecb.AddComponent(beamEntity, new URPMaterialPropertyBaseColor { Value = color });

                                    float3 pos = (p1.position + p2.position) * .5f;
                                    quaternion rot = Quaternion.LookRotation(pointDelta);
                                    float thickness = random.NextFloat(0.25f, 0.35f);
                                    float3 scale = new float3(thickness, thickness, length);
                                    ecb.SetComponent(beamEntity, new Translation { Value = pos });
                                    ecb.SetComponent(beamEntity, new Rotation { Value = rot });
                                    ecb.AddComponent(beamEntity, new NonUniformScale { Value = scale });
                                    ecb.AddComponent(beamEntity, new BeamComponent { Value = beamIndex });

                                    beam.position = pos;
                                    beam.rotation = rot;
                                    beam.length = length;

                                    if (p1.fixedPoint && p2.fixedPoint)
                                    {
                                        beam.fixedBeam = true;
                                        ecb.AddComponent(beamEntity, new FixedTag());
                                    }
                                    else
                                    {
                                        beam.fixedBeam = false;
                                    }

                                    p1.neighbors++;
                                    p2.neighbors++;
                                    anchorPoints[indexP2] = p2;

                                    beams[beamIndex] = beam;
                                    beamCount++;
                                }
                            }
                            anchorPoints[indexP1] = p1;
                        }
                        return beamCount;
                    }

                    // buildings
                    for (int i = 0; i < spawner.BuildingCount; i++)
                    {
                        int height = random.NextInt(4, 12);
                        int anchorStart = anchorPointTotal;
                        int anchorCount = height * 3;
                        float3 pos = new float3(random.NextFloat(-45.0f, 45.0f), 0f, random.NextFloat(-45.0f, 45.0f));
                        for (int j = 0; j < height; j++)
                        {
                            int index = anchorStart + j * 3;
                            AnchorPoint point;
                            point.position = new float3(pos.x + spacing, j * spacing, pos.z - spacing);
                            point.oldPosition = point.position;
                            point.fixedPoint = point.position.y <= 0f;
                            point.neighbors = 0;
                            anchorPoints[index] = point;

                            ++index;
                            point.position = new float3(pos.x - spacing, j * spacing, pos.z - spacing);
                            point.oldPosition = point.position;
                            point.fixedPoint = point.position.y <= 0f;
                            point.neighbors = 0;
                            anchorPoints[index] = point;
                            
                            ++index;
                            point.position = new float3(pos.x - 0f, j * spacing, pos.z + spacing);
                            point.oldPosition = point.position;
                            point.fixedPoint = point.position.y <= 0f;
                            point.neighbors = 0;
                            anchorPoints[index] = point;
                        }
                        int beamStart = beamTotal;
                        int beamCount = generateBeamsForBuilding(anchorStart, anchorCount, beamStart, spawner);
                        
                        //Debug.LogFormat("#{3} Added building{2} group with {0} points and {1} beams.", anchorCount, beamCount, i, buildingCount);
                        
                        var pointEntity = ecb.CreateEntity();
                        ecb.AddComponent(pointEntity, new Building
                        {
                            index = new int4(anchorStart, anchorCount, beamStart, beamCount)
                        });
                        ++buildingCount;

                        anchorPointRealTotal += anchorCount;
                        anchorPointTotal += beamCount * 2;
                        beamTotal += beamCount;
                    }

                    for (int groundSection = 0; groundSection < 9; ++groundSection)
                    {
                        int anchorStart = anchorPointTotal;
                        int anchorCount = 80;
                        for (int i = 0; i < 80; i++)
                        {
                            float4 area = groundSectionAreas[groundSection];
                            float3 pos = new float3(random.NextFloat(area.x, area.y), 0f, random.NextFloat(area.z, area.w));

                            int index = anchorStart + i * 2;
                            AnchorPoint point;
                            point.position = new float3(
                                pos.x + random.NextFloat(-.2f, -.1f),
                                pos.y + random.NextFloat(0f, 3f),
                                pos.z + random.NextFloat(.1f, .2f));
                            point.oldPosition = point.position;
                            point.fixedPoint = point.position.y <= 0f;
                            point.neighbors = 0;
                            anchorPoints[index] = point;
                            
                            ++index;
                            point.position = new float3(
                                pos.x + random.NextFloat(.2f, .1f),
                                pos.y + random.NextFloat(0f, .2f),
                                pos.z + random.NextFloat(-.1f, -.2f));
                            point.oldPosition = point.position;
                            point.fixedPoint = point.position.y <= 0f;
                            point.neighbors = 0;
                            anchorPoints[index] = point;
                        }
                        
                        int beamStart = beamTotal;
                        int beamCount = generateBeamsForBuilding(anchorStart, anchorCount, beamStart, spawner);
                        
                        //Debug.LogFormat("#{2} Added ground beam group with {0} points and {1} beams.", anchorCount, beamCount, buildingCount);
                        
                        var pointEntity = ecb.CreateEntity();
                        ecb.AddComponent(pointEntity, new Building
                        {
                            index = new int4(anchorStart, anchorCount, beamStart, beamCount)
                        });
                        ++buildingCount;
                        
                        anchorPointRealTotal += anchorCount;
                        anchorPointTotal += beamCount * 2;
                        beamTotal += beamCount;
                    }
                }).Run();

            BuildingManager.Instance.Init();
            BuildingManager.AnchorPoints = anchorPoints;
            BuildingManager.Beams = beams;
            
            if (anchorPointRealTotal > 0)
                Debug.LogFormat("Added {0} points (max : {1}, storage: {2}) and {3} beams (storage: {4})", 
                    anchorPointRealTotal, anchorPointTotal, anchorPoints.Length, beamTotal, beams.Length);
        
            ecb.Playback(EntityManager);
            ecb.Dispose();
        }
    }
}