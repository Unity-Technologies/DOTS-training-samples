using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

public class FindClosestFireSystem : SystemBase
{
    private EntityQuery fireQuery;

    protected override void OnCreate()
    {
        RequireSingletonForUpdate<HeatMapTag>();
        fireQuery = GetEntityQuery(
            typeof(Cell),
            typeof(Translation)
        );
    }

    protected override void OnUpdate()
    {
        var fireLocations = fireQuery.ToComponentDataArray<Translation>(Allocator.TempJob);
        Entity heatMapEntity = GetSingletonEntity<HeatMapTag>();
        DynamicBuffer<HeatMap> heatMap = GetBuffer<HeatMap>(heatMapEntity);
        
        Entities
            .WithDisposeOnCompletion(fireLocations)
            .WithAll<LastInLine>()
            .WithAll<FullBucketer>()
            .ForEach((Entity entity, ref TargetPosition nearest, in Translation translation) =>
            {
                float nearestFireDistance = float.MaxValue;
                float3 nearestFireLocation = float3.zero;

                for (int i = 0; i < heatMap.Length; i++)
                {
                    var location = fireLocations[i];
                    var heat = heatMap[i];

                    if (heat.Value < 0.2f)
                    {
                        continue;
                    }

                    var distance = Mathf.Sqrt(
                        Mathf.Pow(translation.Value.x - location.Value.x, 2) +
                        Mathf.Pow(translation.Value.z - location.Value.z, 2));

                    if (distance <= nearestFireDistance)
                    {
                        nearestFireDistance = distance;
                        nearestFireLocation = location.Value;
                    }
                }

                nearest.Value = nearestFireLocation;
            }).Run();
    }
}