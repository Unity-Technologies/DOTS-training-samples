using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class TrafficSpawnerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);

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
                
                Spline spline = new Spline {startPos = pos0, endPos = pos1};
                
                Entity lane0 = ecb.CreateEntity();
                ecb.AddComponent<Lane>(lane0, new Lane{Length = 10.0f, Car = carInstance});
                ecb.AddComponent<Spline>(lane0, spline);
                
                Entity lane1 = ecb.CreateEntity();
                ecb.AddComponent<Lane>(lane1, new Lane{Length = 10.0f});
                ecb.AddComponent<Spline>(lane1, spline);
                
                var intersectionInstance0 = ecb.Instantiate(spawner.SimpleIntersectionPrefab);
                ecb.SetComponent(intersectionInstance0, new SimpleIntersection() {laneIn0 = lane0, laneOut0 = lane1});
                ecb.SetComponent(intersectionInstance0, new Translation{Value = pos0});
                
                var intersectionInstance1 = ecb.Instantiate(spawner.SimpleIntersectionPrefab);
                ecb.SetComponent(intersectionInstance1, new SimpleIntersection() {laneIn0 = lane1, laneOut0 = lane0});
                ecb.SetComponent(intersectionInstance1, new Translation{Value = pos1});

                
            }).Run();

        ecb.Playback(EntityManager);
    }
}