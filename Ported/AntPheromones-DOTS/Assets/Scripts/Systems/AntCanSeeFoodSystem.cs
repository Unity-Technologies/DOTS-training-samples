using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class AntCanSeeFoodSystem : SystemBase
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
            .ForEach((Entity resEntity, in Translation translation) =>
            {
                if (lineOfSightGrid[(((int) translation.Value.y) * boardWidth) + ((int) translation.Value.x)].present)
                {
                    ecb.AddComponent<CanSeeFood>(resEntity);
                }
                
            }).Run();
        
        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}
