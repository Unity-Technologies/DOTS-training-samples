using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class FetcherSystem : SystemBase
{
    // Update is called once per frame
    protected override void OnUpdate()
    {
        Entities
            .WithAll<Fetcher>()
            .ForEach((Entity entity, ref Position position, ref Translation translation) =>
            {
                translation.Value = new float3(position.coord.x, 0.9f, position.coord.y);
            })
            .Schedule();
    }

}
