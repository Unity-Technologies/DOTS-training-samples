using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public unsafe struct UpdatePheromoneBuckets : IJob
{
    [NativeDisableUnsafePtrRestriction] public int* IndexList;
    [NativeDisableUnsafePtrRestriction] public int2* Buckets;
    public int MapSize;
    public int AntCount;
    [DeallocateOnJobCompletion] public NativeArray<Entity> AntEntities;
    [DeallocateOnJobCompletion] public NativeArray<AntComponent> AntComponents;
    [DeallocateOnJobCompletion] public NativeArray<Translation> AntPositions;

    public void Execute()
    {
        var tempBuckets = new List<int>[MapSize];
        for (int i = 0; i < MapSize; i++)
            tempBuckets[i] = new List<int>();

        for (int i = 0; i < AntCount; i++)
        {
            AntComponent ant = AntComponents[i];
            int index = (int)(math.clamp(AntPositions[i].Value.y, 0, 1) * (MapSize - 1));
            tempBuckets[index].Add(ant.index);
        }

        int antIndex = 0;
        int2 range = new int2(0);
        for (int i = 0; i < MapSize; i++)
        {
            foreach (int idx in tempBuckets[i])
            {
                IndexList[antIndex++] = idx;
                range.y++;
            }

            Buckets[i] = range;
            range.x += range.y;
            range.y = 0;
        }
    }
}
