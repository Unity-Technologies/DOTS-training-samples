using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public class TrafficSpawnerSystem : SystemBase
{
    EntityArchetype m_LaneArchetype;
    //public EntityCommandBufferSystem CommandBufferSystem;

    const float laneOffset = 0.5f;

    public bool isReady = false;
    public bool m_SpawnFromGenerator = false;
    public List<Intersection> m_Intersections = new List<Intersection>();
    
    protected override void OnCreate()
    {
        m_LaneArchetype = EntityManager.CreateArchetype(typeof(Lane), typeof(Spline), typeof(CarBufferElement));
        
    }

    private static Entity CreateCar(EntityCommandBuffer ecb, Entity prefab, float position, float3 color)
    {
        Entity carInstance = ecb.Instantiate(prefab);
        ecb.SetComponent(carInstance, new CarPosition {Value = position});
        ecb.SetComponent(carInstance, new CarSpeed {NormalizedValue = 0});
        ecb.AddComponent<URPMaterialPropertyBaseColor>(carInstance, new URPMaterialPropertyBaseColor(){Value = new float4(color.x, color.y, color.z, 1)});
        return carInstance;
    }

    protected override void OnUpdate()
    {
        if (!isReady)
            return;
        
        var ecb = new EntityCommandBuffer(Allocator.TempJob);

        var archetype = m_LaneArchetype;
        
        Random random = Random.CreateFromIndex(1);
        var left = math.left() * laneOffset;
        var right = math.right() * laneOffset;
        var forward = math.forward() * laneOffset;
        var back = math.back() * laneOffset;

        List<Intersection> intersections = m_Intersections;
        bool spawnFromGenerator = m_SpawnFromGenerator;
        
        Entities.WithoutBurst()
            .ForEach((Entity entity, in TrafficSpawner spawner) =>
            {
                // Destroying the current entity is a classic ECS pattern,
                // when something should only be processed once then forgotten.
                ecb.DestroyEntity(entity);

                if (spawnFromGenerator)
                {
                    for (int i = 0; i < intersections.Count; i++)
                    {
                        Intersection intersection = intersections[i];

                        if (intersection.lanes.Count == 2)
                        {
                            //TrackSpline trackSpline = intersection.neighborSplines[0];
                            
                            var deadEnd = ecb.Instantiate(spawner.SimpleIntersectionPrefab);
                            ecb.SetComponent(deadEnd, new SimpleIntersection {laneIn0 = intersection.lanes[0], laneOut0 = intersection.lanes[1]});
                            ecb.SetComponent(deadEnd, new Translation {Value = intersection.position});
                            
                            DynamicBuffer<CarBufferElement> buffer = ecb.AddBuffer<CarBufferElement>(intersection.lanes[0]);
                            DynamicBuffer<Entity> entityBuffer = buffer.Reinterpret<Entity>();
                            
                            entityBuffer.Add(CreateCar(ecb, spawner.CarPrefab, 0,
                                random.NextFloat3(new float3(1, 1.0f, 1.0f))));
                        }
                        else if (intersection.lanes.Count == 4)
                        {
                            var doubleIntersection = ecb.Instantiate(spawner.DoubleIntersectionPrefab);
                            ecb.SetComponent(doubleIntersection, new DoubleIntersection {laneIn0 = intersection.lanes[0], laneOut0 = intersection.lanes[1], laneIn1 = intersection.lanes[2], laneOut1 = intersection.lanes[3]});
                            ecb.SetComponent(doubleIntersection, new Translation {Value = intersection.position});
                        }
                        else if (intersection.lanes.Count == 6)
                        {
                            var tripleIntersection = ecb.Instantiate(spawner.TripleIntersectionPrefab);
                            ecb.SetComponent(tripleIntersection, new TripleIntersection {laneIn0 = intersection.lanes[0], laneOut0 = intersection.lanes[1], laneIn1 = intersection.lanes[2], laneOut1 = intersection.lanes[3], laneIn2 = intersection.lanes[4], laneOut2 = intersection.lanes[5], lane0Direction = -1, lane1Direction = -1, lane2Direction = -1});
                            ecb.SetComponent(tripleIntersection, new Translation {Value = intersection.position});
                        }
                        else
                        {
                            throw new Exception("Not supported");
                        }
                    }
                }
                else
                {
                    float3 pos0 = new float3(-5, 0, 0);
                    float3 pos1 = new float3(0, 0, 0);
                    float3 pos2 = new float3(5, 0, 0);
                    float3 pos3 = new float3(10, 0, 0);
                    float3 pos4 = new float3(5, 0, 5);

                    Spline splineA0 = new Spline {startPos = pos0 + back, endPos = pos1 + back};
                    Spline splineB0 = new Spline {startPos = pos1 + forward, endPos = pos0 + forward};

                    Spline splineA1 = new Spline {startPos = pos1 + back, endPos = pos2 + back};
                    Spline splineB1 = new Spline {startPos = pos2 + forward, endPos = pos1 + forward};

                    Spline splineA2 = new Spline {startPos = pos2 + back, endPos = pos3 + back};
                    Spline splineB2 = new Spline {startPos = pos3 + forward, endPos = pos2 + forward};

                    Spline splineA3 = new Spline {startPos = pos2 + right, endPos = pos4 + right};
                    Spline splineB3 = new Spline {startPos = pos4 + left, endPos = pos2 + left};

                    Entity laneA0 = ecb.CreateEntity(archetype);
                    Lane lane = new Lane {Length = 10.0f};
                    ecb.SetComponent(laneA0, lane);
                    ecb.SetComponent(laneA0, splineA0);
                    DynamicBuffer<CarBufferElement> buffer = ecb.AddBuffer<CarBufferElement>(laneA0);
                    DynamicBuffer<Entity> entityBuffer = buffer.Reinterpret<Entity>();

                    int nbElements = 2;
                    float distance = lane.Length / nbElements;

                    for (int i = 0; i < nbElements; i++)
                    {
                        entityBuffer.Add(CreateCar(ecb, spawner.CarPrefab, lane.Length - i * distance,
                            random.NextFloat3(new float3(1, 1.0f, 1.0f))));
                    }

                    Entity laneB0 = ecb.CreateEntity(archetype);
                    ecb.SetComponent(laneB0, new Lane {Length = 10.0f});
                    ecb.SetComponent(laneB0, splineB0);

                    Entity laneA1 = ecb.CreateEntity(archetype);
                    ecb.SetComponent(laneA1, new Lane {Length = 10.0f});
                    ecb.SetComponent(laneA1, splineA1);

                    Entity laneB1 = ecb.CreateEntity(archetype);
                    ecb.SetComponent(laneB1, new Lane {Length = 10.0f});
                    ecb.SetComponent(laneB1, splineB1);

                    Entity laneA2 = ecb.CreateEntity(archetype);
                    ecb.SetComponent(laneA2, new Lane {Length = 10.0f});
                    ecb.SetComponent(laneA2, splineA2);

                    Entity laneB2 = ecb.CreateEntity(archetype);
                    ecb.SetComponent(laneB2, new Lane {Length = 10.0f});
                    ecb.SetComponent(laneB2, splineB2);

                    Entity laneA3 = ecb.CreateEntity(archetype);
                    ecb.SetComponent(laneA3, new Lane {Length = 10.0f});
                    ecb.SetComponent(laneA3, splineA3);

                    Entity laneB3 = ecb.CreateEntity(archetype);
                    ecb.SetComponent(laneB3, new Lane {Length = 10.0f});
                    ecb.SetComponent(laneB3, splineB3);

                    buffer = ecb.AddBuffer<CarBufferElement>(laneB3);
                    entityBuffer = buffer.Reinterpret<Entity>();

                    for (int i = 0; i < nbElements; i++)
                    {
                        entityBuffer.Add(CreateCar(ecb, spawner.CarPrefab, lane.Length - i * distance,
                            random.NextFloat3(new float3(1, 1.0f, 1.0f))));
                    }

                    var firstDeadEnd = ecb.Instantiate(spawner.SimpleIntersectionPrefab);
                    ecb.SetComponent(firstDeadEnd, new SimpleIntersection {laneIn0 = laneB0, laneOut0 = laneA0});
                    ecb.SetComponent(firstDeadEnd, new Translation {Value = pos0});
                    ecb.AddComponent<URPMaterialPropertyBaseColor>(firstDeadEnd,
                        new URPMaterialPropertyBaseColor() {Value = new float4(1, 0, 0, 1)});

                    var doubleIntersectionInstance0 = ecb.Instantiate(spawner.DoubleIntersectionPrefab);
                    ecb.SetComponent(doubleIntersectionInstance0,
                        new DoubleIntersection
                            {laneIn0 = laneA0, laneOut0 = laneB0, laneIn1 = laneB1, laneOut1 = laneA1});
                    ecb.SetComponent(doubleIntersectionInstance0, new Translation {Value = pos1});

                    var tripleIntersectionInstance0 = ecb.Instantiate(spawner.TripleIntersectionPrefab);
                    ecb.SetComponent(tripleIntersectionInstance0,
                        new TripleIntersection
                        {
                            laneIn0 = laneA1, laneOut0 = laneB1, laneIn1 = laneB2, laneOut1 = laneA2, laneIn2 = laneB3,
                            laneOut2 = laneA3, lane0Direction = -1, lane1Direction = -1, lane2Direction = -1
                        });
                    ecb.SetComponent(tripleIntersectionInstance0, new Translation {Value = pos2});

                    var secondDeadEnd = ecb.Instantiate(spawner.SimpleIntersectionPrefab);
                    ecb.SetComponent(secondDeadEnd, new SimpleIntersection {laneIn0 = laneA2, laneOut0 = laneB2});
                    ecb.SetComponent(secondDeadEnd, new Translation {Value = pos3});
                    ecb.AddComponent<URPMaterialPropertyBaseColor>(secondDeadEnd,
                        new URPMaterialPropertyBaseColor() {Value = new float4(0, 1, 0, 1)});

                    var thirdDeadEnd = ecb.Instantiate(spawner.SimpleIntersectionPrefab);
                    ecb.SetComponent(thirdDeadEnd, new SimpleIntersection {laneIn0 = laneA3, laneOut0 = laneB3});
                    ecb.SetComponent(thirdDeadEnd, new Translation {Value = pos4});
                    ecb.AddComponent<URPMaterialPropertyBaseColor>(thirdDeadEnd,
                        new URPMaterialPropertyBaseColor() {Value = new float4(0, 0, 1, 1)});
                }
            }).Run();
        
        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}
