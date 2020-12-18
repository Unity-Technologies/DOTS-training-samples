using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class AntCanSeeHomeSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entity pheromoneEntity = GetSingletonEntity<Pheromones>();
        int boardWidth = EntityManager.GetComponentData<Board>(pheromoneEntity).BoardWidth;

        Entity homeLineOfSightEntity = GetSingletonEntity<HomeLineOfSightBufferElement>();
        DynamicBuffer<HomeLineOfSightBufferElement> lineOfSightGrid = EntityManager.GetBuffer<HomeLineOfSightBufferElement>(homeLineOfSightEntity);


        var ecb = new EntityCommandBuffer(Allocator.TempJob);
        
        Entities
            .WithAll<Ant, HasFood>()
            .ForEach((Entity resEntity, in Translation translation) =>
            {
                if (lineOfSightGrid[(((int) translation.Value.y) * boardWidth) + ((int) translation.Value.x)].present)
                {
                    ecb.AddComponent<CanSeeHome>(resEntity);
                }
                
            }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}
