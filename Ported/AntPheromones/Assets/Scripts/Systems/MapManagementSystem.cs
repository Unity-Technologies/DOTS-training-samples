using System;

using UnityEngine;

using Unity.Entities;

public class MapManagementSystem : SystemBase
{
    protected override void OnCreate()
    {
        RequireSingletonForUpdate<Map>();
    }

    protected override void OnUpdate()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
            Entities.WithAny<Obstacle, Home, Food>().ForEach((Entity entity) =>
            {
                ecb.DestroyEntity(entity);
            }).Run();
            ecb.Playback(EntityManager);

            Entity mapEntity = GetSingletonEntity<Map>();
            EntityManager.AddComponentData<MapBuilder>(mapEntity, new MapBuilder());
        }
    }
}
