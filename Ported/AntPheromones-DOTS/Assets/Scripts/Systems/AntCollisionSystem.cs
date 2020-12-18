using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

[UpdateAfter(typeof(AntMovementSystem))]
[UpdateBefore(typeof(PheromoneSteeringSystem))]
[UpdateBefore(typeof(AntFoodSteeringSystem))]
[UpdateBefore(typeof(AntHomeSteeringSystem))]
[UpdateBefore(typeof(AntRandomSteeringSystem))]
[UpdateBefore(typeof(AntBoardEdgeInvertSteeringSystem))]
public class AntCollisionSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float time = Time.DeltaTime;
        float timeMultiplier = GetSingleton<TimeMultiplier>().SimulationSpeed;
        float scaledTime = time * timeMultiplier;
		
        Entity obstacleBufferEntity = GetSingletonEntity<ObstacleBufferElement>();
        DynamicBuffer<ObstacleBufferElement> obstacleGrid = EntityManager.GetBuffer<ObstacleBufferElement>(obstacleBufferEntity);

        Entity pheromoneEntity = GetSingletonEntity<Pheromones>();
        int boardWidth = EntityManager.GetComponentData<Board>(pheromoneEntity).BoardWidth;
        
        const float repulsionWeight = 0.1f;
        const float originalDirectionWeight = 1.0f - repulsionWeight;
        
        Entities
            .WithReadOnly(obstacleGrid)
            .WithAll<Ant>()
            .ForEach((ref Heading heading, ref Translation translation) =>
            {
                //Detect collision
                int indexInObstacleGrid = (((int) translation.Value.y) * boardWidth) + ((int) translation.Value.x);

                if (obstacleGrid[indexInObstacleGrid].present)
                {
                    //Find a safe place go
                    int safeIndex = FindSafeSpot(obstacleGrid, indexInObstacleGrid, boardWidth);

                    //int safeIndex = FindSafeSpotViaHeading(obstacleGrid, translation, heading, boardWidth);

                    translation.Value = new float3( safeIndex % boardWidth, safeIndex/boardWidth , 0);
                    
                    heading.heading = -heading.heading;
                }
                
                // for (int i = 0; i < obstacleEntities.Length; ++i)
                // {
                //     var obstacleTranslation = GetComponent<Translation>(obstacleEntities[i]);
                //     var obstacleRadius = GetComponent<Radius>(obstacleEntities[i]);
                //
                //     var antToCollider = translation.Value - obstacleTranslation.Value;
                //     var sqrMag = antToCollider.x * antToCollider.x + antToCollider.y * antToCollider.y;
                //
                //     if (sqrMag <= (obstacleRadius.radius * obstacleRadius.radius))
                //     {
                //         // Collided! Reverse the heading
                //         heading.heading = new float2(heading.heading.x * -1, heading.heading.y * -1);
                //     }
                // }
                
                // float2 repulseDirection = new float();
                // heading.heading = math.normalize((heading.heading * originalDirectionWeight) + (repulseDirection * repulseSteerWeight)); 
            }).ScheduleParallel();
    }
    
    public static int FindSafeSpotViaHeading(DynamicBuffer<ObstacleBufferElement> obstacleGrid, Translation translation, Heading heading, int boardWidth)
    {
        float granularityOfSearch = 0.1f; //one tenth a grid space
        for (int i = 1; i < 100; i++)
        {
            float2 scaledHeading = heading.heading * granularityOfSearch * i;
            float2 stepBack =  new float2(translation.Value.x - scaledHeading.x, translation.Value.y - scaledHeading.y);

            int indexToCheck = (((int) stepBack.y) * boardWidth) + ((int) stepBack.x);
            if (!obstacleGrid[indexToCheck].present) return indexToCheck;
        }
        
        Debug.Log("Something went very wrong");
        return 8000;
    }

    public static int FindSafeSpot(DynamicBuffer<ObstacleBufferElement> obstacleGrid, int indexInObstacleGrid, int boardWidth)
    {

        for (int i = 1; i < 4; i++)
        {
            int up = indexInObstacleGrid + boardWidth * i;
            if (!obstacleGrid[up].present) return up;
            int down = indexInObstacleGrid - boardWidth * i;
            if (!obstacleGrid[down].present) return down;
            int upRight = indexInObstacleGrid + boardWidth * i + i;
            if (!obstacleGrid[upRight].present) return upRight;
            int upLeft = indexInObstacleGrid + boardWidth * i - i;
            if (!obstacleGrid[upLeft].present) return upLeft;
            int downRight = indexInObstacleGrid - boardWidth * i + i;
            if (!obstacleGrid[downRight].present) return downRight;
            int downLeft = indexInObstacleGrid - boardWidth * i - i;
            if (!obstacleGrid[downLeft].present) return downLeft;
            int right = indexInObstacleGrid +  i;
            if (!obstacleGrid[right].present) return right;
            int left = indexInObstacleGrid -  i;
            if (!obstacleGrid[left].present) return left;
        }
        
        Debug.Log("The worst has happened");
        return 8000;
    }
    
}
