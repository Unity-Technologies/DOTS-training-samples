using System;
using Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

public partial struct AntsManagementSystem : ISystem
{
	private uint RandomSeed;
	
	[BurstCompile] 
    public void OnCreate(ref SystemState state)
    {
	    RandomSeed = 1;
        state.RequireForUpdate<Ant>();
        state.RequireForUpdate<Config>();
        state.RequireForUpdate<Pheromone>();
    }
    
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
	    var pheromones = SystemAPI.GetSingletonBuffer<Pheromone>().Reinterpret<short>();
	    if (pheromones.Length <= 0)
	    {
		    return;
	    }
	    
        var random = Unity.Mathematics.Random.CreateFromIndex(RandomSeed++); //this should be centralised

        var config = SystemAPI.GetSingleton<Config>();

        float2 foodPosition = float2.zero, homePosition = float2.zero;

        foreach(var antTarget in SystemAPI.Query<RefRO<AntsTarget>>())
        {
            if (!antTarget.ValueRO.isHome)
                foodPosition = antTarget.ValueRO.position;
            else
                homePosition = antTarget.ValueRO.position;
        }

		/*{
	        //init
	        float2 targetPosition = ant.ValueRO.hasFood ? homePosition : foodPosition;
	        
	        float2 position = ant.ValueRO.position;

	        var facingAngle = ant.ValueRO.facingAngle;

	        float2 directionToTarget;
	        float distanceToTarget;

	        var isTargetVisible = true;
	        float dx, dy, dist;

	        float speed, vx, vy, ovx, ovy;
        }*/

		/*foreach (var ant in SystemAPI.Query<RefRW<Ant>>())
        {


	        float2 position = ant.ValueRO.position;
	        float dx, dy;
	        var facingAngle = ant.ValueRO.facingAngle;
	        
            #region obstacle avoidance
            {
	            foreach (var obstacle in SystemAPI.Query<RefRO<Obstacle>>())
	            {
		            dx = position.x - obstacle.ValueRO.position.x;
		            dy = position.y - obstacle.ValueRO.position.y;
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
					            ant.ValueRW.facingAngle += i * config.WallSteerStrength;
				            }
			            }
		            }
	            }
            }
            #endregion

        }*/

		var obstacleQuerry = SystemAPI.QueryBuilder().WithAll<Obstacle>().Build();
		var obstacles = obstacleQuerry.ToComponentDataArray<Obstacle>(state.WorldUpdateAllocator);

		ObstacleAvoidanceJob obstacleAvoidanceJob = new ObstacleAvoidanceJob() 
		{ config = config, obstacles = obstacles };

		state.Dependency = obstacleAvoidanceJob.ScheduleParallel(state.Dependency);
		state.Dependency.Complete();

        foreach (var ant in SystemAPI.Query<RefRW<Ant>>())
        {
            #region random steering
            {
	            ant.ValueRW.facingAngle += random.NextFloat(-config.RandomSteering, config.RandomSteering);
            }
            #endregion
            
        }

        foreach (var ant in SystemAPI.Query<RefRW<Ant>>())
        {
            #region pheromone steering
            {
	            var pheromoneSteering = PheromoneSteering(ant.ValueRO, 3f, config.MapSize, ref pheromones);
	            ant.ValueRW.facingAngle += pheromoneSteering * config.PheromoneSteering;
            }
            #endregion
            
        }

        foreach (var ant in SystemAPI.Query<RefRW<Ant>>())
        {
			#region line of sight
            {
	            float2 position = ant.ValueRO.position;
	            float2 targetPosition = ant.ValueRO.hasFood ? homePosition : foodPosition;
	            foreach (var obstacle in SystemAPI.Query<RefRO<Obstacle>>())
	            {
		            ant.ValueRW.hasSpottedTarget = !DoLineAndCircleIntersect(obstacle.ValueRO.position, config.ObstacleRadius,
			            position, targetPosition);
		            if (!ant.ValueRO.hasSpottedTarget)
			            break;
	            }
            }
            #endregion
            
        }

        foreach (var ant in SystemAPI.Query<RefRW<Ant>>())
        {
            #region goal steering
            {
	            if (ant.ValueRO.hasSpottedTarget)
	            {
		            float2 targetPosition = ant.ValueRO.hasFood ? homePosition : foodPosition;
		            float2 position = ant.ValueRO.position;
		            var facingAngle = ant.ValueRO.facingAngle;
		            float targetAngle = math.atan2(targetPosition.y - position.y, targetPosition.x - position.x);
		            
		            if (targetAngle - facingAngle > math.PI)
		            {
			            ant.ValueRW.facingAngle += math.PI * 2f;
		            }
		            else if (targetAngle - facingAngle < -math.PI)
		            {
			            ant.ValueRW.facingAngle -= math.PI * 2f;
		            }
		            else if (math.abs(targetAngle - facingAngle) < math.PI * .5f)
		            {
			            ant.ValueRW.facingAngle += (targetAngle - facingAngle) * config.GoalSteerStrength;
		            }
	            }
            }
            #endregion
            
        } 

        foreach (var (ant, transform, color) in SystemAPI.Query<RefRW<Ant>, RefRW<LocalTransform>, RefRW<URPMaterialPropertyBaseColor>>())
        {
	        float speed, vx, vy, ovx, ovy;
	        float dx, dy, dist;
	        
	        float2 position = ant.ValueRO.position;

	        var facingAngle = ant.ValueRO.facingAngle;
            #region movement
            {
	            speed = ant.ValueRO.speed * SystemAPI.Time.DeltaTime;

	            vx = math.cos(facingAngle) * speed;
	            vy = math.sin(facingAngle) * speed;
	            ovx = vx;
	            ovy = vy;

	            if (position.x + vx < 0f || position.x + vx > config.MapSize)
	            {
		            vx = -vx;
	            }

	            position.x += vx;

	            if (position.y + vy < 0f || position.y + vy > config.MapSize)
	            {
		            vy = -vy;
	            }

	            position.y += vy;
            }
            #endregion
            #region target collision reponse
            {
	            float2 targetPosition = ant.ValueRO.hasFood ? homePosition : foodPosition;
	            var directionToTarget = targetPosition - position;
	            var distanceToTarget = math.lengthsq(directionToTarget);
	            if (distanceToTarget < config.TargetRadius * config.TargetRadius)
	            {
		            ant.ValueRW.hasFood = !ant.ValueRO.hasFood;
		            color.ValueRW.Value = ant.ValueRO.hasFood ? config.AntHasFoodColor : config.AntHasNoFoodColor;
		            facingAngle += math.PI;
	            }
            }
            #endregion
            #region obstacle collision response
            {
	            foreach (var obstacle in SystemAPI.Query<RefRO<Obstacle>>())
	            {
		            dx = position.x - obstacle.ValueRO.position.x;
		            dy = position.y - obstacle.ValueRO.position.y;
		            float sqrDist = dx * dx + dy * dy;
		            if (sqrDist < config.ObstacleRadius * config.ObstacleRadius)
		            {
			            dist = math.sqrt(sqrDist);
			            dx /= dist;
			            dy /= dist;
			            position.x = obstacle.ValueRO.position.x + dx * config.ObstacleRadius;
			            position.y = obstacle.ValueRO.position.y + dy * config.ObstacleRadius;

			            vx -= dx * (dx * vx + dy * vy) * 1.5f;
			            vy -= dy * (dx * vx + dy * vy) * 1.5f;
		            }
	            }
            }
            #endregion
            #region post collision
            {
	            if (ovx != vx || ovy != vy)
	            {
		            facingAngle = math.atan2(vy, vx);
	            }
            }
            #endregion
            #region pheromone drop
            {
	            float excitement = .3f;
	            if (ant.ValueRO.hasFood)
	            {
		            excitement = 1f;
	            }

	            //excitement *= ant.ValueRO.speed / antSpeed;
	            DropPheromones(position, excitement, config.MapSize, config.PheromoneAddSpeed, pheromones,
		            SystemAPI.Time.DeltaTime);
            }
            #endregion
            #region write to ant
            {
	            ant.ValueRW.facingAngle = facingAngle;
	            ant.ValueRW.position = position;
            }
            #endregion
            #region update transform
            {
	            transform.ValueRW = LocalTransform.FromPositionRotationScale(
		            new float3(position.x, 0f, position.y),
		            quaternion.AxisAngle(new float3(0, 1, 0), -facingAngle),
		            config.AntRadius * 2);
            }
            #endregion
        }

        float PheromoneSteering(Ant ant, float distance, int mapSize, ref DynamicBuffer<short> pheromones)
        {
	        int output = 0;

            for (int i = -1; i <= 1; i += 2)
            {
                float angle = ant.facingAngle + i * math.PI * .25f;
                float testX = ant.position.x + math.cos(angle) * distance;
                float testY = ant.position.y + math.sin(angle) * distance;

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
            return math.sign(output);
        }

        void DropPheromones(Vector2 position, float strength, int mapSize, float trailAddSpeed,
	        DynamicBuffer<short> pheromones, float deltaTime)
        {
	        int x = (int)math.floor(position.x);
	        int y = (int)math.floor(position.y);
	        if (x < 0 || y < 0 || x >= mapSize || y >= mapSize)
	        {
		        return;
	        }

	        int index = ((mapSize - 1) - x) + ((mapSize - 1) - y) * mapSize;
	        pheromones[index] +=
		        (short)math.floor(trailAddSpeed * short.MaxValue * strength * deltaTime * ((short.MaxValue - pheromones[index]) / (float)short.MaxValue));
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
