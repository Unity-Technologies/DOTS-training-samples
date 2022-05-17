using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;

public class BezierSpawnUtility
{
	public static Entity SpawnOnBezier(Entity entity, float distance, Entity trackEntity, EntityCommandBuffer ecb)
	{
        var rail = ecb.Instantiate(entity);
        ecb.AddComponent<DistanceAlongBezier>(rail, new DistanceAlongBezier { Distance = distance, TrackEntity = trackEntity});
        return rail;
    }
}
