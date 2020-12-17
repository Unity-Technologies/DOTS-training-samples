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
        DynamicBuffer<Pheromones> pheromoneGrid = EntityManager.GetBuffer<Pheromones>(pheromoneEntity);
        int boardWidth = EntityManager.GetComponentData<Board>(pheromoneEntity).BoardWidth;

        Entity lineOfSightEntity = GetSingletonEntity<LineOfSightBufferElement>();
        DynamicBuffer<LineOfSightBufferElement> lineOfSightGrid = EntityManager.GetBuffer<LineOfSightBufferElement>(lineOfSightEntity);

        var ecb = new EntityCommandBuffer(Allocator.TempJob);
        
        Entities
            .WithAll<Ant>()
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
