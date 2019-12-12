
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public struct AntResolvePositionJob : IJobForEach<Translation, AntComponent>
{
	[ReadOnly] public int2 Dimensions;
	[ReadOnly] public NativeArray<int2> ObstacleBuckets;
	[ReadOnly] public NativeArray<MapObstacle> CachedObstacles;

	private int2 GetObstacleBucket(float3 position)
	{
		int2 cell = new int2(math.clamp((int)(position.x * Dimensions.x), 0, (Dimensions.x - 1)),
							 math.clamp((int)(position.y * Dimensions.y), 0, (Dimensions.y - 1)));
		int bucketIndex = (cell.y * Dimensions.x) + cell.x;
		int2 range = ObstacleBuckets[bucketIndex];

		return range;
	}
	
	public void Execute(ref Translation translation, ref AntComponent ant)
	{
		float3 antPosition = translation.Value;
		float facingAngle = ant.facingAngle;
		float speed = ant.speed;
		
		math.sincos(facingAngle, out float sin, out float cos);
		
		float3 velocity = new float3(speed * cos, speed * sin, 0.0f);
		int2 range = GetObstacleBucket(antPosition);

		if(range.y > 0)
		{
			for (int j = 0; j < range.y; j++)
			{
				MapObstacle obstacle = CachedObstacles[j + range.x];
				float obstacleRadius = obstacle.radius;
				float3 delta = antPosition - obstacle.position;
				float sqrDist = math.lengthsq(delta);
			
				if (sqrDist < (obstacleRadius * obstacleRadius))
				{
					float dist = Mathf.Sqrt(sqrDist);
				
					delta /= dist;
					antPosition = obstacle.position + (delta * obstacleRadius);
					velocity -= delta * math.dot(delta, velocity) * 1.5f;
				}
			}

			translation.Value = antPosition;
			ant.facingAngle = math.atan2(velocity.y, velocity.x);
			ant.speed = math.length(velocity);
		}
	}
}
