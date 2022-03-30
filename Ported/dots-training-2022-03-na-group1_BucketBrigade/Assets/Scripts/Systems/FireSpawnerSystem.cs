using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Collections;

public partial class FireSpawnerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        
        Entities
            .ForEach((Entity entity, in Spawner spawner) =>
            {

            }).Run();
        
        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}
