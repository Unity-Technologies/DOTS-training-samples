
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
	
	private int WallSteering(AntComponent ant, float3 position, float distance) {
		int output = 0;

		for (int i = -1; i <= 1; i+=2) {
			float angle = ant.facingAngle + i * math.PI*.25f;
			math.sincos(angle, out float sin, out float cos);
			float testX = position.x + cos * distance;
			float testY = position.y + sin * distance;

			if (testX < 0 || testY < 0 || testX >= 1.0f || testY >= 1.0f) {

			} else {
				int value = GetObstacleBucket(testX, testY).y;
				if (value > 0) {
					output -= i;
				}
			}
		}
		return output;
	}


	public void Execute([ReadOnly] ref AntComponent ant, [ReadOnly] ref Translation antTranslation, ref AntSteeringComponent antSteering)
	{
		antSteering.WallSteering = WallSteering(ant, antTranslation.Value, LookAheadDistance);
	}
}
