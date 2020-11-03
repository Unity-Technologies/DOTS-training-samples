using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class TrafficSpawnerSystem : SystemBase
{
    EntityArchetype m_LaneArchetype; 
    
    protected override void OnCreate()
    {
        m_LaneArchetype = EntityManager.CreateArchetype(typeof(Lane), typeof(Spline));
    }

    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        var archetype = m_LaneArchetype;
        
        Entities
            .ForEach((Entity entity, in TrafficSpawner spawner) =>
            {
                // Destroying the current entity is a classic ECS pattern,
                // when something should only be processed once then forgotten.
                ecb.DestroyEntity(entity);

                var carInstance = ecb.Instantiate(spawner.CarPrefab);
                ecb.SetComponent(carInstance, new CarPosition {Value = 3.0f});

                float3 pos0 = new float3(-5, 0, 0);
                float3 pos1 = new float3(5, 0, 0);
                float3 pos2 = new float3(15, 0, 0);
                
                Spline spline0 = new Spline {startPos = pos0, endPos = pos1};
                Spline spline1 = new Spline {startPos = pos1, endPos = pos2};
                Spline spline2 = new Spline {startPos = pos2, endPos = pos1};
                Spline spline3 = new Spline {startPos = pos1, endPos = pos0};
                
                Entity lane0 = ecb.CreateEntity(archetype);
                ecb.SetComponent(lane0, new Lane{Length = 10.0f, Car = carInstance});
                ecb.SetComponent(lane0, spline0);
                
                Entity lane1 = ecb.CreateEntity(archetype);
                ecb.SetComponent(lane1, new Lane{Length = 10.0f});
                ecb.SetComponent(lane1, spline1);
                
                Entity lane2 = ecb.CreateEntity(archetype);
                ecb.SetComponent(lane2, new Lane{Length = 10.0f});
                ecb.SetComponent(lane2, spline2);
                
                Entity lane3 = ecb.CreateEntity(archetype);
                ecb.SetComponent(lane3, new Lane{Length = 10.0f});
                ecb.SetComponent(lane3, spline3);
                
                var intersectionInstance0 = ecb.Instantiate(spawner.SimpleIntersectionPrefab);
                ecb.SetComponent(intersectionInstance0, new SimpleIntersection {laneIn0 = lane3, laneOut0 = lane0});
                ecb.SetComponent(intersectionInstance0, new Translation{Value = pos0});
                
                var intersectionInstance1 = ecb.Instantiate(spawner.SimpleIntersectionPrefab);
                ecb.SetComponent(intersectionInstance1, new SimpleIntersection {laneIn0 = lane1, laneOut0 = lane2});
                ecb.SetComponent(intersectionInstance1, new Translation{Value = pos2});
                
                var doubleIntersectionInstance0 = ecb.Instantiate(spawner.DoubleIntersectionPrefab);
                ecb.SetComponent(doubleIntersectionInstance0, new DoubleIntersection {laneIn0 = lane0, laneOut0 = lane1, laneIn1 = lane2, laneOut1 = lane3});
                ecb.SetComponent(doubleIntersectionInstance0, new Translation{Value = pos1});

                
            }).Run();

        ecb.Playback(EntityManager);
    }
}