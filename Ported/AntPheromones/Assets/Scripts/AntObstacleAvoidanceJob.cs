
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
public struct AntObstacleAvoidanceJob : IJobForEach<AntComponent, Translation, AntSteeringComponent>
{
	[ReadOnly] public int2 Dimensions;
	[ReadOnly] public NativeArray<int2> ObstacleBuckets;
	[ReadOnly] public NativeArray<MapObstacle> CachedObstacles;
	public float LookAheadDistance;

	private int2 GetObstacleBucket(float3 position)
	{
		int2 cell = new int2(math.clamp((int)(position.x * Dimensions.x), 0, (Dimensions.x - 1)),
							 math.clamp((int)(position.y * Dimensions.y), 0, (Dimensions.y - 1)));
		int bucketIndex = (cell.y * Dimensions.x) + cell.x;
		int2 range = ObstacleBuckets[bucketIndex];

		return range;
	}
	
	public void Execute([ReadOnly] ref AntComponent ant, [ReadOnly] ref Translation antTranslation, ref AntSteeringComponent antSteering)
	{
		math.sincos(ant.facingAngle, out float sin, out float cos);

		float3 antPosition = antTranslation.Value;
		float3 lookAheadDirection = new float3(cos, sin, 0.0f);
		float3 perpendicularDirection = new float3(sin, -cos, 0.0f);
		float3 lookAheadPosition = antPosition + (lookAheadDirection * LookAheadDistance);
		int2 range = GetObstacleBucket(lookAheadPosition);
		float steeringValue = 0.0f;

		// Loop through all obstacles in our bucket...
		for(int i = 0; i < range.y; i++)
		{
			MapObstacle cachedObstacle = CachedObstacles[range.x + i];
			float distance2 = math.distancesq(cachedObstacle.position, lookAheadPosition);

			float radius = 1.5f * cachedObstacle.radius;
			if(distance2 < radius * radius)
			{
				float3 directionToObstacle = math.normalize(cachedObstacle.position - lookAheadPosition);
				float dotProduct = math.dot(perpendicularDirection, directionToObstacle);
				float angleDelta = dotProduct;
			
				// We've found an obstacle - steer away from it...
				steeringValue += math.sign(angleDelta);
			}
		}

		antSteering.WallSteering = steeringValue;
	}
}
