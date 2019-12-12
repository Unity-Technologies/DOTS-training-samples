
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
public struct AntObstacleAvoidanceJob : IJobForEach<AntComponent, Translation>
{
	[ReadOnly] private int2 m_Dimensions;
	[ReadOnly] private NativeArray<int2> m_ObstacleBuckets;
	[ReadOnly] private NativeArray<RuntimeManager.CachedObstacle> m_CachedObstacles;

	private int2 GetObstacleBucket(float3 position)
	{
		int2 cell = new int2(math.clamp((int)(position.x * m_Dimensions.x), 0, (m_Dimensions.x - 1)),
							 math.clamp((int)(position.y * m_Dimensions.y), 0, (m_Dimensions.y - 1)));
		int bucketIndex = (cell.y * m_Dimensions.x) + cell.x;
		int2 range = m_ObstacleBuckets[bucketIndex];

		return range;
	}

	public AntObstacleAvoidanceJob(int2 dimensions, NativeArray<int2> obstacleBuckets, NativeArray<RuntimeManager.CachedObstacle> cachedObstacles)
	{
		m_Dimensions = dimensions;
		m_ObstacleBuckets = obstacleBuckets;
		m_CachedObstacles = cachedObstacles;
	}
	
	public void Execute(ref AntComponent ant, ref Translation antTranslation)
	{
		math.sincos(ant.facingAngle, out float sin, out float cos);

		float3 antPosition = antTranslation.Value;
		float lookAheadDistance = 0.01f;
		float3 lookAheadPosition = antPosition + new float3(cos * lookAheadDistance, sin * lookAheadDistance, 0.0f);
		int2 range = GetObstacleBucket(lookAheadPosition);

		for(int i = 0; i < range.y; i++)
		{
			RuntimeManager.CachedObstacle cachedObstacle = m_CachedObstacles[range.x + i];
			float distance = math.distance(cachedObstacle.position, lookAheadPosition);

			if(distance < cachedObstacle.radius)
			{
				float3 testDirection = new float3(sin, -cos, 0.0f);
				float3 directionToObstacle = cachedObstacle.position - lookAheadPosition;
				float dotProduct = math.dot(testDirection, directionToObstacle);
				float angleDelta = dotProduct;
				
				// We've found an obstacle - steer away from it...
				ant.facingAngle += angleDelta;
				ant.speed = 0.0f;
			}
		}
	}
}
