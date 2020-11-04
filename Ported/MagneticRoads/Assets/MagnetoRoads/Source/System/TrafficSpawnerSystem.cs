using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using Random = Unity.Mathematics.Random;

public class TrafficSpawnerSystem : SystemBase
{
    EntityArchetype m_LaneArchetype;
    //public EntityCommandBufferSystem CommandBufferSystem;

    const float laneOffset = 0.5f;
    
    protected override void OnCreate()
    {
        m_LaneArchetype = EntityManager.CreateArchetype(typeof(Lane), typeof(Spline), typeof(MyBufferElement));
        //CommandBufferSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        
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
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        var archetype = m_LaneArchetype;
        
        //EntityCommandBuffer commandBuffer = CommandBufferSystem.CreateCommandBuffer();
        Entity carInstance;
        Entity laneA0;

        Random random = Random.CreateFromIndex(0);
        var left = math.left() * laneOffset;
        var right = math.right() * laneOffset;
        var forward = math.forward() * laneOffset;
        var back = math.back() * laneOffset;
        
        Entities
            .ForEach((Entity entity, in TrafficSpawner spawner) =>
            {
                // Destroying the current entity is a classic ECS pattern,
                // when something should only be processed once then forgotten.
                ecb.DestroyEntity(entity);

                /*var carInstance2 = ecb.Instantiate(spawner.CarPrefab);
                ecb.SetComponent(carInstance2, new CarPosition {Value = 3.0f});
                ecb.AddComponent<URPMaterialPropertyBaseColor>(carInstance2, new URPMaterialPropertyBaseColor(){Value = new float4(1, 0, 0, 1)});*/

                float3 pos0 = new float3(-5, 0, 0);
                float3 pos1 = new float3(0, 0, 0);
                float3 pos2 = new float3(5, 0, 0);
                float3 pos3 = new float3(10, 0, 0);
                float3 pos4 = new float3(5, 0, 5);
                
                Spline splineA0 = new Spline {startPos = pos0 + back, endPos = pos1 + back};
                Spline splineB0 = new Spline {startPos = pos1 + forward, endPos = pos0 + forward};
                
                Spline splineA1 = new Spline {startPos = pos1 + back , endPos = pos2 + back};
                Spline splineB1 = new Spline {startPos = pos2 + forward, endPos = pos1 + forward};
                
                Spline splineA2 = new Spline {startPos = pos2 + back, endPos = pos3 + back};
                Spline splineB2 = new Spline {startPos = pos3 + forward, endPos = pos2 + forward};
                
                Spline splineA3 = new Spline {startPos = pos2 + right, endPos = pos4 + right};
                Spline splineB3 = new Spline {startPos = pos4 + left, endPos = pos2 + left};
                
                laneA0 = ecb.CreateEntity(archetype);
                Lane lane = new Lane {Length = 10.0f};
                ecb.SetComponent(laneA0, lane);
                ecb.SetComponent(laneA0, splineA0);
                DynamicBuffer<MyBufferElement> buffer = ecb.AddBuffer<MyBufferElement>(laneA0);
                DynamicBuffer<Entity> entityBuffer = buffer.Reinterpret<Entity>();

                int nbElements = 2;
                float distance = lane.Length/nbElements;

                for (int i = 0; i < nbElements; i++)
                {
                    entityBuffer.Add(CreateCar(ecb, spawner.CarPrefab, lane.Length - i * distance, random.NextFloat3(new float3(1,1.0f, 1.0f))));
                }
                
                //var buffer = lookup[laneA0];
                //buffer.Add(carInstance);
                
                Entity laneB0 = ecb.CreateEntity(archetype);
                ecb.SetComponent(laneB0, new Lane{Length = 10.0f});
                ecb.SetComponent(laneB0, splineB0);
                //ecb.AddBuffer<MyBufferElement>(laneB0);
                
                Entity laneA1 = ecb.CreateEntity(archetype);
                ecb.SetComponent(laneA1, new Lane{Length = 10.0f});
                ecb.SetComponent(laneA1, splineA1);
                //ecb.AddBuffer<MyBufferElement>(laneA1);
                
                Entity laneB1 = ecb.CreateEntity(archetype);
                ecb.SetComponent(laneB1, new Lane{Length = 10.0f});
                ecb.SetComponent(laneB1, splineB1);
                //ecb.AddBuffer<MyBufferElement>(laneB1);
                
                Entity laneA2 = ecb.CreateEntity(archetype);
                ecb.SetComponent(laneA2, new Lane{Length = 10.0f});
                ecb.SetComponent(laneA2, splineA2);
                //ecb.AddBuffer<MyBufferElement>(laneA2);
                
                Entity laneB2 = ecb.CreateEntity(archetype);
                ecb.SetComponent(laneB2, new Lane{Length = 10.0f});
                ecb.SetComponent(laneB2, splineB2);
                //ecb.AddBuffer<MyBufferElement>(laneB2);
                
                Entity laneA3 = ecb.CreateEntity(archetype);
                ecb.SetComponent(laneA3, new Lane{Length = 10.0f});
                ecb.SetComponent(laneA3, splineA3);
                //ecb.AddBuffer<MyBufferElement>(laneA3);
                
                Entity laneB3 = ecb.CreateEntity(archetype);
                ecb.SetComponent(laneB3, new Lane{Length = 10.0f});
                ecb.SetComponent(laneB3, splineB3);
                //ecb.AddBuffer<MyBufferElement>(laneB3);
                
                buffer = ecb.AddBuffer<MyBufferElement>(laneB3);
                entityBuffer = buffer.Reinterpret<Entity>();
                
                for (int i = 0; i < nbElements; i++)
                {
                    entityBuffer.Add(CreateCar(ecb, spawner.CarPrefab, lane.Length - i * distance, random.NextFloat3(new float3(1,1.0f, 1.0f))));
                }
                
                var firstDeadEnd = ecb.Instantiate(spawner.SimpleIntersectionPrefab);
                ecb.SetComponent(firstDeadEnd, new SimpleIntersection {laneIn0 = laneB0, laneOut0 = laneA0});
                ecb.SetComponent(firstDeadEnd, new Translation{Value = pos0});
                ecb.AddComponent<URPMaterialPropertyBaseColor>(firstDeadEnd, new URPMaterialPropertyBaseColor(){Value = new float4(1, 0, 0, 1)});
                
                var doubleIntersectionInstance0 = ecb.Instantiate(spawner.DoubleIntersectionPrefab);
                ecb.SetComponent(doubleIntersectionInstance0, new DoubleIntersection {laneIn0 = laneA0, laneOut0 = laneB0, laneIn1 = laneB1, laneOut1 = laneA1});
                ecb.SetComponent(doubleIntersectionInstance0, new Translation{Value = pos1});
                
                var tripleIntersectionInstance0 = ecb.Instantiate(spawner.TripleIntersectionPrefab);
                ecb.SetComponent(tripleIntersectionInstance0, new TripleIntersection {laneIn0 = laneA1, laneOut0 = laneB1, laneIn1 = laneB2, laneOut1 = laneA2, laneIn2 = laneB3, laneOut2 = laneA3, lane0Direction = -1, lane1Direction = -1, lane2Direction = -1});
                ecb.SetComponent(tripleIntersectionInstance0, new Translation{Value = pos2});
                
                var secondDeadEnd = ecb.Instantiate(spawner.SimpleIntersectionPrefab);
                ecb.SetComponent(secondDeadEnd, new SimpleIntersection {laneIn0 = laneA2, laneOut0 = laneB2});
                ecb.SetComponent(secondDeadEnd, new Translation{Value = pos3});
                ecb.AddComponent<URPMaterialPropertyBaseColor>(secondDeadEnd, new URPMaterialPropertyBaseColor(){Value = new float4(0, 1, 0, 1)});
                
                var thirdDeadEnd = ecb.Instantiate(spawner.SimpleIntersectionPrefab);
                ecb.SetComponent(thirdDeadEnd, new SimpleIntersection {laneIn0 = laneA3, laneOut0 = laneB3});
                ecb.SetComponent(thirdDeadEnd, new Translation{Value = pos4});
                ecb.AddComponent<URPMaterialPropertyBaseColor>(thirdDeadEnd, new URPMaterialPropertyBaseColor(){Value = new float4(0, 0, 1, 1)});
            }).Run();
        
        ecb.Playback(EntityManager);
        //CommandBufferSystem.AddJobHandleForProducer(this.Dependency);
    }
}
