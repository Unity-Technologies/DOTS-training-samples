using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public struct DropPheromonesJob : IJobParallelFor
{
    [ReadOnly] public AntSettings Settings;
    [ReadOnly] public NativeArray<int2> Buckets;
    [ReadOnly] public NativeArray<int> IndexList;
    [ReadOnly] public NativeArray<AntOutput> AntOutput;
    [NativeDisableParallelForRestriction] public NativeArray<float> PheromoneMap;

    public void Execute(int index)
    {
        var b = Buckets[index];
        for (int j = b.x; j < b.x + b.y; j++)
        {
            var o = AntOutput[IndexList[j]];

            if (o.pos < 0 || o.pos >= Settings.mapSize)
            {
                continue;
            }
            PheromoneMap[index * Settings.mapSize + o.pos] = o.value;
        }
    }

}
