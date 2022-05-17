using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;

public class BezierSpawnUtility
{
	public static Entity SpawnOnBezier(Entity entity, NativeArray<BezierPoint> bezier, float distAsPercentage, EntityCommandBuffer ecb)
	{
        float3 posOnRail = BezierPath.Get_Position(bezier, distAsPercentage);
        float3 tangentOnRail = BezierPath.Get_NormalAtPosition(bezier, distAsPercentage);

        var rotation = Quaternion.LookRotation(tangentOnRail);

        var rail = ecb.Instantiate(entity);
        ecb.SetComponent(rail, new Translation { Value = posOnRail });
        ecb.SetComponent(rail, new Rotation { Value = rotation });
        return rail;
    }
}
