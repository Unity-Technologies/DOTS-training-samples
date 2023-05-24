using System;
using Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

public partial struct AntsManagementSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Ant>();
        state.RequireForUpdate<Config>();
    }

    public void OnUpdate(ref SystemState state)
    {
        var random = Unity.Mathematics.Random.CreateFromIndex((uint)DateTime.UtcNow.Ticks); //this should be centralised

        var config = SystemAPI.GetSingleton<Config>();

        float2 foodPosition = float2.zero, homePosition = float2.zero;

        foreach(var antTarget in SystemAPI.Query<RefRO<AntsTarget>>())
        {
            if (!antTarget.ValueRO.isHome)
                foodPosition = antTarget.ValueRO.position;
            else
                homePosition = antTarget.ValueRO.position;
        }

        foreach (var (ant, transform, color) in SystemAPI.Query<RefRW<Ant>, RefRW<LocalTransform>, RefRW<URPMaterialPropertyBaseColor>>())
        {
            var targetPosition = ant.ValueRO.hasFood ? homePosition : foodPosition;
            var position = ant.ValueRO.position;

            //creating a var rwAnt = ant.ValueRW and updating this resulted in the changes not propagating up to the actual components

            var directionToTarget = targetPosition - position;
            var distanceToTarget = math.distancesq(directionToTarget.x, directionToTarget.y);
            var toTargetAngle = math.atan2(directionToTarget.y, directionToTarget.x);

            var facingAngle = ant.ValueRO.facingAngle;

            facingAngle += random.NextFloat(-config.RandomSteering, config.RandomSteering);/// * SystemAPI.Time.DeltaTime; //steering


            //facingAngle = toTargetAngle;

            if (distanceToTarget < config.ObstacleRadius)
            {
                ant.ValueRW.hasFood = !ant.ValueRO.hasFood;
                //color.ValueRW.Value = ant.ValueRO.hasFood ? config.AntHasFoodColor : config.AntHasNoFoodColor;
            }

            var speed = ant.ValueRO.speed * SystemAPI.Time.DeltaTime;

            float vx = math.cos(facingAngle) * speed;
            float vy = math.sin(facingAngle) * speed;
            float ovx = vx;
			float ovy = vy;

            
            if (position.x + vx < 0f || position.x + vx > config.MapSize)
            {
                vx = -vx;
                //facingAngle += math.PI; //this should model reflection, not inversion (as it is here)
            }

            position.x += vx;

            if (position.y + vy < 0f || position.y + vy > config.MapSize) // since the direction of vy is reversed, this logic needs revisited
            {
                vy = -vy;
                //facingAngle += math.PI; //this should model reflection, not inversion (as it is here)
            }

            position.y += vy; //this used to be += vy

			float dx, dy, dist;

            foreach (var obstacle in SystemAPI.Query<RefRO<Obstacle>>())
            {
			//for (int j=0;j<nearbyObstacles.Length;j++) {
				dx = position.x - obstacle.ValueRO.position.x;
				dy = position.y - obstacle.ValueRO.position.y;
				float sqrDist = dx * dx + dy * dy;
				if (sqrDist<config.ObstacleRadius*config.ObstacleRadius) {
					dist = math.sqrt(sqrDist);
					dx /= dist;
					dy /= dist;
					position.x = obstacle.ValueRO.position.x + dx * config.ObstacleRadius;
					position.y = obstacle.ValueRO.position.y + dy * config.ObstacleRadius;

					vx -= dx * (dx * vx + dy * vy) * 1.5f;
					vy -= dy * (dx * vx + dy * vy) * 1.5f;
				}
			}

			if (ovx != vx || ovy != vy) {
				facingAngle = math.atan2(vy,vx);
			}

            //I'm not sure that this is the most efficient way, but it intuitively feels like it to me
            ant.ValueRW.facingAngle = facingAngle;
            ant.ValueRW.position = position;

            transform.ValueRW = LocalTransform.FromPositionRotationScale(
                    new float3(position.x, 0f, position.y),
                    quaternion.AxisAngle(new float3(0, 1, 0), -facingAngle),
                    config.ObstacleRadius * 2);
        }
    }
}
