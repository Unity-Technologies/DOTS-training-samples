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
