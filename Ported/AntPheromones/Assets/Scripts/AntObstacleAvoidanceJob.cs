
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

	public AntObstacleAvoidanceJob(int2 dimensions, NativeArray<int2> obstacleBuckets, NativeArray<RuntimeManager.CachedObstacle> cachedObstacles)
	{
		m_Dimensions = dimensions;
		m_ObstacleBuckets = obstacleBuckets;
		m_CachedObstacles = cachedObstacles;
	}
	
	public void Execute(ref AntComponent ant, ref Translation antTranslation)
	{
		int2 cell = new int2(math.clamp((int)(antTranslation.Value.x * m_Dimensions.x), 0, (m_Dimensions.x - 1)),
							 math.clamp((int)(antTranslation.Value.y * m_Dimensions.y), 0, (m_Dimensions.y - 1)));
		int bucketIndex = (cell.y * m_Dimensions.x) + cell.x;
		int2 range = m_ObstacleBuckets[bucketIndex];

		for(int i = 0; i < range.y; i++)
		{
			RuntimeManager.CachedObstacle cachedObstacle = m_CachedObstacles[range.x + i];
			float distance = math.distance(cachedObstacle.position, antTranslation.Value);

			if(distance < cachedObstacle.radius)
			{
				ant.speed = 0.0f;
			}
			// float normalizedDistance = distance / cachedObstacle.radius;
			// float clamped = 1.0f - math.clamp(normalizedDistance, 0.0f, 1.0f);
			//
			// ant.speed *= clamped;
		}
	}
}
