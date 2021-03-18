using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UIElements;

public class FindClosestLastInFillupLineSystem : SystemBase
{
    private EntityQuery LastInLineQuery;
    
    protected override void OnCreate()
    {
        LastInLineQuery = GetEntityQuery(
            typeof(LastInLine),
            typeof(EmptyBucketer),
            typeof(Translation));
    }

    protected override void OnUpdate()
    {
        var linebucketerPositions = LastInLineQuery.ToComponentDataArray<Translation>(Allocator.TempJob);

        Entities
            .WithDisposeOnCompletion(linebucketerPositions)
            .WithAll<BucketFetcher, CarryingBucket>()
            .ForEach((Entity entity, ref TargetPosition targetPosition,in Translation pos, in Speed speed) =>
            {
                var bucketId = GetComponent<BucketID>(entity);
                
                // if bucket empty, find nearest water source with line
                if (bucketId.Value != Entity.Null)
                {
                    var bucketVol = GetComponent<Volume>(bucketId.Value);
                    if (bucketVol.Value < 1.0f)
                    {
                        int minIndex = linebucketerPositions.Length;
                        var minDist = float.MaxValue;
                        for (int i = 0; i < linebucketerPositions.Length; i++)
                        {
                            var currentDist = Unity.Mathematics.math.distance(linebucketerPositions[i].Value, pos.Value);
                            if (currentDist < minDist)
                            {
                                minDist = currentDist;
                                minIndex = i;
                            }
                        }

                        if (minIndex != linebucketerPositions.Length)
                        {
                            var newTarget = new float3(linebucketerPositions[minIndex].Value);
                            newTarget.x += 0.5f;
                            targetPosition.Value = newTarget;
                        }
                    }
                }
            }).Run();
    }
}
