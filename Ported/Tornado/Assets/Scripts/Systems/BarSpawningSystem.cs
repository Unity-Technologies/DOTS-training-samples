using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class BarSpawningSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

        //Entities.ForEach(Entity entity, in BarSpawner spawner ) => { }.Run();
    }
}
