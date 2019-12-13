using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
public struct AntResolvePositionJob : IJobForEach<Translation, AntComponent>
{
	[ReadOnly] public int2 Dimensions;
	[ReadOnly] public NativeArray<int2> ObstacleBuckets;
	[ReadOnly] public NativeArray<MapObstacle> CachedObstacles;

	private int2 GetObstacleBucket(float2 position)
	{
		int2 cell = math.clamp((int2)(position * Dimensions), 0, Dimensions - 1);
		int bucketIndex = (cell.y * Dimensions.x) + cell.x;
		int2 range = ObstacleBuckets[bucketIndex];

		return range;
	}
	
	public void Execute(ref Translation translation, ref AntComponent ant)
	{
		float2 antPosition = translation.Value.xy;
		float facingAngle = ant.facingAngle;
		float speed = ant.speed;
		
		math.sincos(facingAngle, out float sin, out float cos);
		
		float2 velocity = new float2(speed * cos, speed * sin);
		int2 range = GetObstacleBucket(antPosition);

		if(range.y > 0)
		{
			for (int j = 0; j < range.y; j++)
			{
				MapObstacle obstacle = CachedObstacles[j + range.x];
				float obstacleRadius = obstacle.radius;
				float2 delta = antPosition - obstacle.position;
				float sqrDist = math.lengthsq(delta);
			
				if (sqrDist < (obstacleRadius * obstacleRadius))
				{
					float dist = math.sqrt(sqrDist);
				
					delta /= dist;
					antPosition = obstacle.position + (delta * obstacleRadius);
					velocity -= delta * math.dot(delta, velocity) * 1.5f;
				}
			}

			translation.Value = new float3(antPosition,0);
			ant.facingAngle = math.atan2(velocity.y, velocity.x);
			ant.speed = math.length(velocity);
		}
	}
}
