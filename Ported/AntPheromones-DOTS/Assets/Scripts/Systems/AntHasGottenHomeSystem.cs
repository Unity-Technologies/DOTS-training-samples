using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class AntHasGottenHomeSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entity goalEntity = GetSingletonEntity<Home>();
        Translation goalTranslation= EntityManager.GetComponentData<Translation>(goalEntity);

        var ecb = new EntityCommandBuffer(Allocator.TempJob);
        
        Entities
            .WithAll<Ant, HasFood, CanSeeHome>()
            .ForEach((Entity resEntity, ref Heading heading, in Translation translation) =>
            {
                var antFromHome = translation.Value - goalTranslation.Value;
                var sqrMag = antFromHome.x * antFromHome.x + antFromHome.y * antFromHome.y;

                if (sqrMag <= 7) 
                {
                    ecb.RemoveComponent<HasFood>(resEntity);
                    ecb.RemoveComponent<CanSeeHome>(resEntity);
                    heading.heading = -heading.heading;
                }
                
            }).Run();
        
        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}
