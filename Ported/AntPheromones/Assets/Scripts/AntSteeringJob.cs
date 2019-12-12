
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Collections;
using UnityEngine;

[BurstCompile]
public struct AntSteeringJob : IJobForEach<Translation, AntComponent, AntSteeringComponent>
{
    [ReadOnly] public int MapSize;
    [ReadOnly] [NativeDisableParallelForRestriction] [DeallocateOnJobCompletion] public NativeArray<float> RandomDirections;
    [ReadOnly] [NativeDisableParallelForRestriction] public NativeArray<float> PheromoneMap;

    private readonly static float lookAheadDistance = 3.0f;

    int PheromoneIndex(int x, int y) 
    {
		return x + y * MapSize;
	}

    float PheromoneSteering(in AntComponent ant, in Translation translation) 
    {
		float output = 0;

		for (int i = -1; i <= 1; i += 2) 
        {
            float angle = ant.facingAngle + i * Mathf.PI * 0.25f;
			float testX = translation.Value.x + Mathf.Cos(angle) * lookAheadDistance;
			float testY = translation.Value.y + Mathf.Sin(angle) * lookAheadDistance;

			if (testX < 0 || testY < 0 || testX >= MapSize || testY >= MapSize) 
                continue;

            int index = PheromoneIndex((int)testX, (int)testY);
            float value = PheromoneMap[index];
            output += value * i;
		}

		return Mathf.Sign(output);
	}

	public void Execute([ReadOnly] ref Translation translation, [ReadOnly] ref AntComponent ant, ref AntSteeringComponent antSteering)
	{
		antSteering.RandomDirection = RandomDirections[ant.index];
		antSteering.PheromoneSteering = PheromoneSteering(ant, translation);
	}
}
