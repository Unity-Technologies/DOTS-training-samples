using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class ApplyCanSeeFoodComponentSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entity pheromoneEntity = GetSingletonEntity<Pheromones>();
        int boardWidth = EntityManager.GetComponentData<Board>(pheromoneEntity).BoardWidth;

        Entity lineOfSightEntity = GetSingletonEntity<GoalLineOfSightBufferElement>();
        DynamicBuffer<GoalLineOfSightBufferElement> lineOfSightGrid = EntityManager.GetBuffer<GoalLineOfSightBufferElement>(lineOfSightEntity);

        var ecb = new EntityCommandBuffer(Allocator.TempJob);
        
        Entities
            .WithAll<Ant>()
            .WithNone<HasFood, CanSeeFood>()
            .ForEach((Entity resEntity, ref Ant ant) =>
            {
                if (ant.shouldGetLineOfSight)
                {
                    ecb.AddComponent<CanSeeFood>(resEntity);
                    ant.shouldGetLineOfSight = false;
                }
            }).Run();
        
        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}
