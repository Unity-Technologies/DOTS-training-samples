using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class CarPositionToWorldSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        Entities
            .ForEach((Entity entity, ref CarPosition carPosition, ref Spline spline, ref Lane lane) =>
            {
                float splineRatio = carPosition.Value / lane.length;
                float3 pos = math.lerp(spline.startPos, spline.endPos, splineRatio);
                Translation translation = new Translation {Value = pos}; 
                ecb.SetComponent(entity, translation);
            }).Run();

        ecb.Playback(EntityManager);
    }
}