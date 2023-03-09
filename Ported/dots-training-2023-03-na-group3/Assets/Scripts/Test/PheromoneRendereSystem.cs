using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

using Unity.Collections.LowLevel.Unsafe;

public partial class pheromoneRendereSystem : SystemBase
{
    private PheromoneRenderer map;

    protected override void OnUpdate()
    {
        var pheromoneData = SystemAPI.GetSingletonBuffer<PheromoneData>();
        
        if (map == null)
        {
            map = GameObject.Find("Map").GetComponent<PheromoneRenderer>();
        }

        NativeArray<float> data = new NativeArray<float>(pheromoneData.Length, Allocator.Temp);
        for (var i = 0; i < pheromoneData.Length; i++)
        {
            data[i] = pheromoneData[i].value;
        }
        map.SetTextureData(data);
    }
}
