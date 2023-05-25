using Components;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[BurstCompile]
public partial struct ObstacleAvoidanceJob : IJobEntity
{
	public Config config;
	[ReadOnly] public NativeArray<Obstacle> obstacles;

    public void Execute(ref Ant ant)
    {
		float2 position = ant.position;
		float dx, dy;
		var facingAngle = ant.facingAngle;

		#region obstacle avoidance
		{
			foreach (var obstacle in obstacles)
			{
				dx = position.x - obstacle.position.x;
				dy = position.y - obstacle.position.y;
				float sqrDist = dx * dx + dy * dy;
				if (sqrDist < config.SqrSteeringDist)
				{
					for (int i = -1; i <= 1; i += 2)
					{
						float angle = facingAngle + i * math.PI * .25f;
						float testX = position.x + math.cos(angle) * config.SteeringDist;
						float testY = position.y + math.sin(angle) * config.SteeringDist;

						if (testX < 0 || testY < 0 || testX >= config.MapSize || testY >= config.MapSize)
						{

						}
						else
						{
							ant.facingAngle += i * config.WallSteerStrength;
						}
					}
				}
			}
		}
		#endregion
	}
}

[BurstCompile]
public partial struct RandomSteeringJob : IJobEntity
{
	public Config config;
	public Unity.Mathematics.Random random;
	
	public void Execute(ref Ant ant)
    {
		ant.facingAngle += random.NextFloat(-config.RandomSteering, config.RandomSteering);
	}
}

[BurstCompile]
public partial struct PheromoneSteeringJob : IJobEntity
{
	public Config config;
	[ReadOnly] public DynamicBuffer<short> pheromones;

	public void Execute(ref Ant ant)
	{
		int output = 0;
		int distance = 3;

		for (int i = -1; i <= 1; i += 2)
		{
			float angle = ant.facingAngle + i * math.PI * .25f;
			float testX = ant.position.x + math.cos(angle) * distance;
			float testY = ant.position.y + math.sin(angle) * distance;

			if (testX < 0 || testY < 0 || testX >= config.MapSize || testY >= config.MapSize)
			{

			}
			else
			{
				int index = ((config.MapSize - 1) - (int)math.floor(testX)) + ((config.MapSize - 1) - (int)math.floor(testY)) * config.MapSize;
				short value = pheromones[index];
				output += value * i;
			}
		}
		var pheromoneSteering = math.sign(output);

		ant.facingAngle += pheromoneSteering * config.PheromoneSteering;
	}
}


[BurstCompile]
public partial struct LineOfSightJob : IJobEntity
{
	public Config config;
	public float2 homePosition, foodPosition;
	[ReadOnly] public NativeArray<Obstacle> obstacles;

	public void Execute(ref Ant ant)
	{
		float2 position = ant.position;
		float2 targetPosition = ant.hasFood ? homePosition : foodPosition;

		#region line of sight
		{
			foreach (var obstacle in obstacles)
			{
				ant.hasSpottedTarget = !DoLineAndCircleIntersect(obstacle.position, config.ObstacleRadius,
					position, targetPosition);
				if (!ant.hasSpottedTarget)
					break;
			}
		}
		#endregion
	}
	
	bool DoLineAndCircleIntersect(float2 circlePosition, float circleRadius, float2 lineStart, float2 lineEnd)
	{
		float2 AC = circlePosition - lineStart;
		float2 AB = lineEnd - lineStart;
		float ab2 = AB.x * AB.x + AB.y * AB.y;// + AB.z * AB.z;
		float acab = math.dot(AC, AB);
		float t = acab / ab2;
		if (t < 0)
			t = 0;
		else if (t > 1)
			t = 1;
		float2 H = new float2(lineStart.x + AB.x * t, lineStart.y + AB.y * t);
		float h2 = (H.x - circlePosition.x) * (H.x - circlePosition.x) + (H.y - circlePosition.y) * (H.y - circlePosition.y);
		return h2 <= circleRadius * circleRadius;
	}
}


[BurstCompile]
public partial struct GoalSteeringJob : IJobEntity
{
	public Config config;
	public float2 homePosition, foodPosition;
	
	public void Execute(ref Ant ant)
	{
		if (ant.hasSpottedTarget)
		{
			float2 targetPosition = ant.hasFood ? homePosition : foodPosition;
			float2 position = ant.position;
			var facingAngle = ant.facingAngle;
			float targetAngle = math.atan2(targetPosition.y - position.y, targetPosition.x - position.x);
		            
			if (targetAngle - facingAngle > math.PI)
			{
				ant.facingAngle += math.PI * 2f;
			}
			else if (targetAngle - facingAngle < -math.PI)
			{
				ant.facingAngle -= math.PI * 2f;
			}
			else if (math.abs(targetAngle - facingAngle) < math.PI * .5f)
			{
				ant.facingAngle += (targetAngle - facingAngle) * config.GoalSteerStrength;
			}
		}
	}
}