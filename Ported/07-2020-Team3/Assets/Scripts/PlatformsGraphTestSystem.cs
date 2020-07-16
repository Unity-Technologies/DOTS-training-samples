using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class PlatformsGraphTestSystem : SystemBase
{
    protected override void OnUpdate()
    {
        return;
        var myBlobData = GetSingleton<PlatformsGraph>().Resolve;
        Entities
            .WithAll<Commuter>()
            .ForEach((ref Translation translation) =>
            {
                translation.Value = new float3(myBlobData.SomeFloat, 0, 0);
            }).Schedule();
    }
}
