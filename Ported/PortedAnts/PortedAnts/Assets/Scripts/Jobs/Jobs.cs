using Components;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
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
							ant.deltaSteering += i;
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
				var steer = value * i;
				output += steer;
				ant.deltaSteering += steer;
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

[BurstCompile]
public partial struct AntsMovementJob : IJobEntity
{
	public Config config;
	public float deltaTime;
	
	public void Execute(ref Ant ant)
	{
		float speed = ant.speed * deltaTime;

		ant.vx = math.cos(ant.facingAngle) * speed;
		ant.vy = math.sin(ant.facingAngle) * speed;
		ant.ovx = ant.vx;
		ant.ovy = ant.vy;

		if (ant.position.x + ant.vx < 0f || ant.position.x + ant.vx > config.MapSize)
		{
			ant.vx = -ant.vx;
		}

		ant.position.x += ant.vx;

		if (ant.position.y + ant.vy < 0f || ant.position.y + ant.vy > config.MapSize)
		{
			ant.vy = -ant.vy;
		}

		ant.position.y += ant.vy;
	}
}

[BurstCompile]
public partial struct TargetCollisionJob : IJobEntity
{
	public Config config;
	public float2 homePosition, foodPosition;

	public void Execute(ref Ant ant, ref URPMaterialPropertyBaseColor color)
	{
		float2 targetPosition = ant.hasFood ? homePosition : foodPosition;
		var directionToTarget = targetPosition - ant.position;
		var distanceToTarget = math.lengthsq(directionToTarget);
		if (distanceToTarget < config.TargetRadius * config.TargetRadius)
		{
			ant.hasFood = !ant.hasFood;
			color.Value = ant.hasFood ? config.AntHasFoodColor : config.AntHasNoFoodColor;
			ant.facingAngle += math.PI;
		}
	}
}

[BurstCompile]
public partial struct UpdateTransformJob : IJobEntity
{
	public Config config;
	
	public void Execute(Ant ant, ref LocalTransform transform)
	{
		transform = LocalTransform.FromPositionRotationScale(
			new float3(ant.position.x, 0f, ant.position.y),
			quaternion.AxisAngle(new float3(0, 1, 0), -ant.facingAngle),
			config.AntRadius * 2);
	}
}

[BurstCompile]
public partial struct PostCollisionUpdateJob : IJobEntity
{
	public void Execute(ref Ant ant)
	{
		if (ant.ovx != ant.vx || ant.ovy != ant.vy)
		{
			ant.facingAngle = math.atan2(ant.vy, ant.vx);
		}
	}
}

[BurstCompile]
public partial struct ObstacleCollisionResponseJob : IJobEntity
{
	public Config config;
	[ReadOnly] public NativeArray<Obstacle> obstacles;
	
	public void Execute(ref Ant ant)
	{
		float dx, dy, dist;
		foreach (var obstacle in obstacles)
		{
			dx = ant.position.x - obstacle.position.x;
			dy = ant.position.y - obstacle.position.y;
			float sqrDist = dx * dx + dy * dy;
			if (sqrDist < config.ObstacleRadius * config.ObstacleRadius)
			{
				dist = math.sqrt(sqrDist);
				dx /= dist;
				dy /= dist;
				ant.position.x = obstacle.position.x + dx * config.ObstacleRadius;
				ant.position.y = obstacle.position.y + dy * config.ObstacleRadius;

				ant.vx -= dx * (dx * ant.vx + dy * ant.vy) * 1.5f;
				ant.vy -= dy * (dx * ant.vx + dy * ant.vy) * 1.5f;
			}
		}
	}
}

[BurstCompile]
public struct PheromoneUpdateJob : IJobParallelFor
{
	public Config config;
	public DynamicBuffer<Pheromone> pixels;
	public void Execute(int i)
	{
		pixels[i] = new Pheromone() { Value = (short)(pixels[i].Value * config.PheromoneDecay) };
	}
}