using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;

public class BezierSpawnUtility
{
	public static Entity SpawnOnBezier(Entity entity, int trackIndex, float distAsPercentage, EntityCommandBuffer ecb)
	{
        var rail = ecb.Instantiate(entity);
        ecb.AddComponent<PositionOnBezier>(rail, new PositionOnBezier { Position = distAsPercentage, BezierIndex = trackIndex });
        return rail;
    }
}
