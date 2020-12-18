using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class AntReachedFoodSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entity goalEntity = GetSingletonEntity<Goal>();
        Translation goalTranslation= EntityManager.GetComponentData<Translation>(goalEntity);

        var ecb = new EntityCommandBuffer(Allocator.TempJob);
        
        Entities
            .WithAll<Ant, CanSeeFood>()
            .WithNone<HasFood>()
            .ForEach((Entity resEntity, ref Heading heading, ref Translation translation) =>
            {
                var antFromHome = translation.Value - goalTranslation.Value;
                var sqrMag = antFromHome.x * antFromHome.x + antFromHome.y * antFromHome.y;

                if (sqrMag <= 7) 
                {
                    ecb.AddComponent<HasFood>(resEntity);
                    ecb.RemoveComponent<CanSeeFood>(resEntity);
                    heading.heading = -heading.heading;
                    
                    translation.Value.x += heading.heading.x;    //step back one whole unit
                    translation.Value.y += heading.heading.y;
                }
                
            }).Run();
        
        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}
