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
            var distanceToTarget = math.lengthsq(directionToTarget);
            var toTargetAngle = math.atan2(directionToTarget.y, directionToTarget.x);

            var isTargetVisible = true;

            foreach (var obstacle in SystemAPI.Query<RefRO<Obstacle>>())
            {
                isTargetVisible = !DoLineAndCircleIntersect(obstacle.ValueRO.position, config.ObstacleRadius, position, targetPosition);
                if (!isTargetVisible)
                    break;
            }

            

            var pheromones = SystemAPI.GetSingletonBuffer<Pheromone>().Reinterpret<short>();
            var pheromoneSteering = PheromoneSteering(ant.ValueRO, 3f, config.MapSize, ref pheromones);

            var facingAngle = ant.ValueRO.facingAngle;

            facingAngle += random.NextFloat(-config.RandomSteering, config.RandomSteering);/// * SystemAPI.Time.DeltaTime; //steering
            facingAngle += pheromoneSteering * config.PheromoneSteering;

            if (isTargetVisible)
                facingAngle = toTargetAngle;

            if (distanceToTarget < config.TargetRadius * config.TargetRadius)
            {
                ant.ValueRW.hasFood = !ant.ValueRO.hasFood;
                color.ValueRW.Value = ant.ValueRO.hasFood ? config.AntHasFoodColor : config.AntHasNoFoodColor;
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
			
			float excitement = .3f;
			if (ant.ValueRO.hasFood) {
				excitement = 1f;
			}
			
			
			//excitement *= ant.ValueRO.speed / antSpeed;
			DropPheromones(position, excitement, config.MapSize, config.PheromoneAddSpeed, ref pheromones);

            //I'm not sure that this is the most efficient way, but it intuitively feels like it to me
            ant.ValueRW.facingAngle = facingAngle;
            ant.ValueRW.position = position;

            transform.ValueRW = LocalTransform.FromPositionRotationScale(
                    new float3(position.x, 0f, position.y),
                    quaternion.AxisAngle(new float3(0, 1, 0), -facingAngle),
                    config.AntRadius * 2);
        }

        float PheromoneSteering(Ant ant, float distance, int mapSize, ref DynamicBuffer<short> pheromones)
        {
            if (PheromoneTextureView.PheromoneTex == null) return 0;

            int output = 0;

            for (int i = -1; i <= 1; i += 2)
            {
                float angle = ant.facingAngle + i * Mathf.PI * .25f;
                float testX = ant.position.x + Mathf.Cos(angle) * distance;
                float testY = ant.position.y + Mathf.Sin(angle) * distance;

                if (testX < 0 || testY < 0 || testX >= mapSize || testY >= mapSize)
                {

                }
                else
                {
                    int index = ((mapSize - 1) - (int)math.floor(testX)) + ((mapSize - 1) - (int)math.floor(testY)) * mapSize;
                    short value = pheromones[index];
                    output += value * i;
                }
            }
            return Mathf.Sign(output);
        }

        void DropPheromones(Vector2 position, float strength, int mapSize, float trailAddSpeed,
	        ref DynamicBuffer<short> pheromones)
        {
	        if (PheromoneTextureView.PheromoneTex == null) return;

	        int x = (int)math.floor(position.x);
	        int y = (int)math.floor(position.y);
	        if (x < 0 || y < 0 || x >= mapSize || y >= mapSize)
	        {
		        return;
	        }

	        int index = ((mapSize - 1) - x) + ((mapSize - 1) - y) * mapSize;
	        pheromones[index] +=
		        (short)math.floor(trailAddSpeed * short.MaxValue * strength * Time.fixedDeltaTime * ((short.MaxValue - pheromones[index]) / (float)short.MaxValue));
	        if (pheromones[index] > short.MaxValue - 1)
	        {
		        pheromones[index] = short.MaxValue;
	        }
        }

        /*bool Linecast(float2 point1, float2 point2)
        {
            float dx = point2.x - point1.x;
            float dy = point2.y - point1.y;
            float dist = Mathf.Sqrt(dx * dx + dy * dy);

            int stepCount = Mathf.CeilToInt(dist * .5f);
            for (int i = 0; i < stepCount; i++)
            {
                float t = (float)i / stepCount;
                if (GetObstacleBucket(point1.x + dx * t, point1.y + dy * t).Length > 0)
                {
                    return true;
                }
            }

            return false;
        }*/

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
}
