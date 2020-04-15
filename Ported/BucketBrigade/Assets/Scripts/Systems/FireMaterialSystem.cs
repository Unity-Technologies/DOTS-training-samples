using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class FireMaterialSystem : SystemBase
{
    protected override void OnUpdate()
    {

        var ecb = new EntityCommandBuffer(Allocator.Temp);
        Entities.ForEach((Entity spawnerEntity, ref FireMaterialComponent material, in ValueComponent val) =>
        {
            //material[]
            // float amount  = val.Value
            ecb.SetComponent(spawnerEntity, new FireMaterialComponent { Amount = 0 });


        }).Run();

        ecb.Playback(EntityManager);
    }
}
