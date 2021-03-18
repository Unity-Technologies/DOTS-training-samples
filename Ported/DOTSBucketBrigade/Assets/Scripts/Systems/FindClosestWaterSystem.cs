using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

public class FindClosestWaterSystem : SystemBase
{
    private EntityQuery waterQuery;
    
    protected override void OnCreate()
    {
        waterQuery = GetEntityQuery(
            typeof(River),
            typeof(Volume),
            typeof(Translation));
    }

    protected override void OnUpdate()
    {
        var waterLocations = waterQuery.ToComponentDataArray<Translation>(Allocator.TempJob);
        var waterVolume = waterQuery.ToComponentDataArray<Volume>(Allocator.TempJob);

        Entities
            .WithDisposeOnCompletion(waterLocations)
            .WithDisposeOnCompletion(waterVolume)
            .WithAll<LastInLine>()
            .WithAll<EmptyBucketer>()
            .ForEach((Entity entity, ref TargetPosition nearest, in Translation translation) =>
            {
                float nearestWaterDistance = float.MaxValue;
                float3 nearestWaterLocation = float3.zero;
                for (int i = 0; i < waterLocations.Length; i++)
                {
                    var location = waterLocations[i];
                    var volume = waterVolume[i];

                    if (volume.Value <= 0.0f)
                    {
                        continue;
                    }

                    var distance = Mathf.Sqrt(
                        Mathf.Pow(translation.Value.x - location.Value.x, 2) +
                        Mathf.Pow(translation.Value.z - location.Value.z, 2));

                    if (distance <= nearestWaterDistance)
                    {
                        nearestWaterDistance = distance;
                        nearestWaterLocation = location.Value;
                    }
                }

                nearest.Value = nearestWaterLocation;
            }).Schedule();
    }
}