using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class FetcherSystem : SystemBase
{
    // Update is called once per frame
    protected override void OnUpdate()
    {
        Entities
            .WithAll<Fetcher>()
            .ForEach((ref Position position) =>
            {
            })
            .Schedule();
    }

}
