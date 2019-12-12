
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Collections;
using UnityEngine;
using Unity.Mathematics;

[BurstCompile]
public struct AntSteeringJob : IJobForEach<Translation, AntComponent, AntSteeringComponent>
{
    [ReadOnly] public float3 ColonyPosition;
    [ReadOnly] public float3 ResourcePosition; 
    [ReadOnly] public int MapSize;
    [ReadOnly] [NativeDisableParallelForRestriction] [DeallocateOnJobCompletion] public NativeArray<float> RandomDirections;
    [ReadOnly] [NativeDisableParallelForRestriction] public NativeArray<float> PheromoneMap;
    [ReadOnly] public int2 ObstacleBucketDimensions;
    [ReadOnly] public NativeArray<int2> ObstacleBuckets;
    [ReadOnly] public NativeArray<RuntimeManager.CachedObstacle> CachedObstacles;

    private readonly static float lookAheadDistance = 3.0f;

    int PheromoneIndex(int x, int y) 
    {
		return x + y * MapSize;
	}

    private int2 GetObstacleBucket(float3 position)
	{
		int2 cell = new int2(math.clamp((int)(position.x * ObstacleBucketDimensions.x), 0, (ObstacleBucketDimensions.x - 1)),
							 math.clamp((int)(position.y * ObstacleBucketDimensions.y), 0, (ObstacleBucketDimensions.y - 1)));
		int bucketIndex = (cell.y * ObstacleBucketDimensions.x) + cell.x;
		int2 range = ObstacleBuckets[bucketIndex];

		return range;
	}

    bool Linecast(float3 point1, float3 point2) 
    {
		float dx = point2.x - point1.x;
		float dy = point2.y - point1.y;
		float dist = Mathf.Sqrt(dx * dx + dy * dy);

		int stepCount = Mathf.CeilToInt(dist*.5f);
		
        for (int s = 0; s < stepCount; s++) 
        {
			float t = (float)s / stepCount;

            float3 lookAheadPosition = new float3(point1.x + dx*t, point1.y + dy*t, 0);

            int2 range = GetObstacleBucket(lookAheadPosition);

            // if there are any obstacles in this bucket
            if (range.y > 0) 
            {
                return true;
            }
		}

		return false;
	}

    float PheromoneSteering(ref AntComponent ant, ref Translation translation) 
    {
		float output = 0;

		for (int i = -1; i <= 1; i += 2) 
        {
            float angle = ant.facingAngle + i * Mathf.PI * 0.25f;
			float testX = translation.Value.x + Mathf.Cos(angle) * lookAheadDistance/MapSize;
			float testY = translation.Value.y + Mathf.Sin(angle) * lookAheadDistance/MapSize;

			if (testX < 0 || testY < 0 || testX >= MapSize || testY >= MapSize) 
                continue;

            int index = PheromoneIndex((int)testX, (int)testY);
            float value = PheromoneMap[index];
            output += value * i;
		}

		return Mathf.Sign(output);
	}

    float TargetSteering(ref AntComponent ant, ref Translation translation)
    {
        var targetPos = ant.state == 0 ? ResourcePosition : ColonyPosition;

        // target is occluded
        if (Linecast(translation.Value, targetPos))
            return 0.0f;

        float targetAngle = Mathf.Atan2(targetPos.y - translation.Value.y,targetPos.x - translation.Value.x);
        
        if (targetAngle - ant.facingAngle > Mathf.PI) 
        {
            return Mathf.PI * 2f;
        } 
        else if (targetAngle - ant.facingAngle < -Mathf.PI) 
        {
            return -Mathf.PI * 2f;
        } 
        else 
        {
            if (Mathf.Abs(targetAngle - ant.facingAngle) < Mathf.PI * 0.5f)
                return (targetAngle - ant.facingAngle);
        }

        return 0.0f;
    }

	public void Execute([ReadOnly] ref Translation translation, [ReadOnly] ref AntComponent ant, ref AntSteeringComponent antSteering)
	{
		antSteering.RandomDirection = RandomDirections[ant.index];
		antSteering.PheromoneSteering = PheromoneSteering(ref ant, ref translation);
        antSteering.TargetSteering = TargetSteering(ref ant, ref translation);
	}
}
