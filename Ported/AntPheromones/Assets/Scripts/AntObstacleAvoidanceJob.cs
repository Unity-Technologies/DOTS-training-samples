//#define SIMPLE_CHECK

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

	private int2 GetObstacleBucket(float x, float y)
	{
		int2 cell = new int2(math.clamp((int)(x * Dimensions.x), 0, (Dimensions.x - 1)),
							 math.clamp((int)(y * Dimensions.y), 0, (Dimensions.y - 1)));
		int bucketIndex = (cell.y * Dimensions.x) + cell.x;
		int2 range = ObstacleBuckets[bucketIndex];

		return range;
	}

	private bool ValidPosition(float2 position)
	{
		if (position.x < 0 || position.y < 0 || position.x >= 1.0f || position.y >= 1.0f)
			return false;

		int2 range = GetObstacleBucket(position.x, position.y);

#if !SIMPLE_CHECK
		for (int j = 0; j < range.y; ++j)
		{
			int obstacleIndex = range.x + j;
			var obstacle = CachedObstacles[obstacleIndex];
			
			if (math.lengthsq(position - obstacle.position) >= obstacle.radius * obstacle.radius)
			{
				return false;
			}
		}
#else
		if (range.y > 0)
			return false;
#endif

		return true;
	}

	private float WallSteering(AntComponent ant, float3 position, float distance) 
	{
		float steering = 0.0f;
		float2 newPosition = 0;

		for (int i = -1; i <= 1; i+=2) 
		{
			float correctionAngle = i * math.PI * 0.25f;
			float angle = ant.facingAngle + correctionAngle;

			float2 dp;
			math.sincos(angle, out dp.x, out dp.y);

			newPosition = position.xy + dp * distance;

			if (math.any(newPosition < 0) || math.any(newPosition >= 1))
				continue;

			if (!ValidPosition(newPosition))
				steering -= correctionAngle;
		}

		return steering;
	}

	public void Execute([ReadOnly] ref AntComponent ant, [ReadOnly] ref Translation antTranslation, ref AntSteeringComponent antSteering)
	{
		antSteering.WallSteering = WallSteering(ant, antTranslation.Value, LookAheadDistance);
	}
}
