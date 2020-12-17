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
        Entity homeEntity = GetSingletonEntity<Home>();
        Translation homeTranslation= EntityManager.GetComponentData<Translation>(homeEntity);

        var ecb = new EntityCommandBuffer(Allocator.TempJob);
        
        Entities
            .WithAll<Ant>()
            .ForEach((Entity resEntity, in Translation translation) =>
            {
                var antFromHome = translation.Value - homeTranslation.Value;
                var sqrMag = antFromHome.x * antFromHome.x + antFromHome.y * antFromHome.y;

                if (sqrMag >= 2700) //approximation of 52 sqaured - 52 being the distance to getting outside the outer ring
                {
                    ecb.AddComponent<CanSeeFood>(resEntity);
                }
                
            }).Run();
        
        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}
