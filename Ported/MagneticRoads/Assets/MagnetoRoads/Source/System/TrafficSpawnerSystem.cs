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

                var instance = ecb.Instantiate(spawner.CarPrefab);
                var carPosition = new CarPosition {Value = 3.0f};
                ecb.SetComponent(instance, carPosition);
            }).Run();

        ecb.Playback(EntityManager);
    }
}