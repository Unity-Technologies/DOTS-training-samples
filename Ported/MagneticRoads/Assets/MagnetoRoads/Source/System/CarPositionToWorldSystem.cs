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
        Entities
            .ForEach((Entity entity, ref Spline spline, ref Lane lane) =>
            {
                if (lane.Car != Entity.Null)
                {
                    CarPosition carPosition = GetComponent<CarPosition>(lane.Car);
                    float splineRatio = carPosition.Value / lane.Length;
                    float3 pos = math.lerp(spline.startPos, spline.endPos, splineRatio);
                    Translation translation = new Translation {Value = pos}; 
                    SetComponent(lane.Car, translation);    
                }
            }).Run();
    }
}
