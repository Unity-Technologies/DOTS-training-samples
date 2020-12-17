using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public class InitializationSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        var random = new Random(2465);
        var center = new Translation {Value = new float3(64, 64, 0)};
        var minRange = new float2(-1, -1);
        var maxRange = new float2(1, 1);
        var bottomLeftFood = new Translation {Value = new float3(10, 10, 0)};
        var topLeftFood = new Translation {Value = new float3(10, 118, 0)};
        var bottomRightFood = new Translation {Value = new float3(118, 10, 0)};
        var topRightFood = new Translation {Value = new float3(118, 118, 0)};

        Entity obstacleEntity = GetSingletonEntity<ObstacleBufferElement>();
        DynamicBuffer<ObstacleBufferElement> obstacleGrid =
            EntityManager.GetBuffer<ObstacleBufferElement>(obstacleEntity);

        Entity lineOfSightEntity = GetSingletonEntity<LineOfSightBufferElement>();
        DynamicBuffer<LineOfSightBufferElement> lineOfSightGrid =
            EntityManager.GetBuffer<LineOfSightBufferElement>(lineOfSightEntity);

        Entity homeLineOfSightEntity = GetSingletonEntity<HomeLineOfSightBufferElement>();
        DynamicBuffer<HomeLineOfSightBufferElement> homeLineOfSightGrid =
            EntityManager.GetBuffer<HomeLineOfSightBufferElement>(homeLineOfSightEntity);
        
        Entities
            .WithoutBurst()
            .ForEach((Entity entity, in Init init) =>
            {
                ecb.DestroyEntity(entity);

                // Create Board
                var board = ecb.Instantiate(init.boardPrefab);
                ecb.SetComponent(board, center);

                // Create Ants
                for (var i = 0; i < init.antCount; i++)
                {
                    var ant = ecb.Instantiate(init.antPrefab);
                    ecb.SetComponent(ant, center);

                    ecb.SetComponent(ant, new Heading
                    {
                        heading = math.normalize(random.NextFloat2(minRange, maxRange))
                    });
                }

                // Create Home
                var home = ecb.Instantiate(init.homePrefab);
                ecb.SetComponent(home, center);

                // Create Food
                var randomFoodPosIndex = 0;// random.NextInt(0, 4);
                var food = ecb.Instantiate(init.goalPrefab);
                switch (randomFoodPosIndex)
                {
                    case 0:
                        ecb.SetComponent(food, bottomLeftFood);
                        break;
                    case 1:
                        ecb.SetComponent(food, topLeftFood);
                        break;
                    case 2:
                        ecb.SetComponent(food, bottomRightFood);
                        break;
                    case 3:
                        ecb.SetComponent(food, topRightFood);
                        break;
                }

                // Create Obstacles
                for (int i = 1; i <= 3; i++)
                {
                    float ringRadius = (i / (3 + 1f)) * (128 * 0.5f);
                    float circumference = ringRadius * 2f * math.PI;
                    float obstacleRadius = 1;
                    int maxCount = Mathf.CeilToInt(circumference / (2f * obstacleRadius));
                    int gapAngle = random.NextInt(0, 300);
                    int gapSize = random.NextInt(30, 60);
                    for (int j = 0; j < maxCount; j++)
                    {
                        float angle = (j) / (float) maxCount * (2f * Mathf.PI);
                        if (angle * Mathf.Rad2Deg >= gapAngle && angle * Mathf.Rad2Deg < gapAngle + gapSize)
                        {
                            continue;
                        }

                        var obstacle = ecb.Instantiate(init.obstaclePrefab);
                        var translation = new Translation
                        {
                            Value = new float3(64f + math.cos(angle) * ringRadius,
                                64f + math.sin(angle) * ringRadius, 0)
                        };

                        int indexInObstacleGrid = (((int) translation.Value.y) * 128) + ((int) translation.Value.x);

                        obstacleGrid[indexInObstacleGrid] = new ObstacleBufferElement {present = true};
                        int boardWidth = 128;

                        int up = indexInObstacleGrid + boardWidth;
                        obstacleGrid[up] = new ObstacleBufferElement {present = true};
                        int down = indexInObstacleGrid - boardWidth;
                        obstacleGrid[down] = new ObstacleBufferElement {present = true};
                        int upRight = indexInObstacleGrid + boardWidth + 1;
                        obstacleGrid[upRight] = new ObstacleBufferElement {present = true};
                        int upLeft = indexInObstacleGrid + boardWidth - 1;
                        obstacleGrid[upLeft] = new ObstacleBufferElement {present = true};
                        int downRight = indexInObstacleGrid - boardWidth + 1;
                        obstacleGrid[downRight] = new ObstacleBufferElement {present = true};
                        int downLeft = indexInObstacleGrid - boardWidth - 1;
                        obstacleGrid[downLeft] = new ObstacleBufferElement {present = true};
                        int right = indexInObstacleGrid + 1;
                        obstacleGrid[right] = new ObstacleBufferElement {present = true};
                        int left = indexInObstacleGrid - 1;
                        obstacleGrid[left] = new ObstacleBufferElement {present = true};

                        ecb.SetComponent(obstacle, translation);
                        ecb.SetComponent(obstacle, new Radius {radius = 4});
                    }
                }
                
                // create Food LineOfSight grid
                UpdateLineInDirection(bottomLeftFood, obstacleGrid, lineOfSightGrid, 128);
                
                // create Home LineOfSight Grid
                UpdateHomeLineInDirection(center, obstacleGrid, homeLineOfSightGrid, 128);
            }).Run();

        ecb.Playback(EntityManager);

        ecb.Dispose();
    }

    public static void UpdateLineInDirection(Translation start, DynamicBuffer<ObstacleBufferElement> obstacleGrid,
        DynamicBuffer<LineOfSightBufferElement> lineOfSightGrid, int boardWidth)
    {
        int[] lineIndices = new int[boardWidth];
        for (int i = 0; i < 360; i++)
        {
            float2 angle = new float2( math.cos(i),  math.sin(i));
            for (int dist = 1; dist < boardWidth; dist++)
            {
                float2 posToCheck = new float2(start.Value.x, start.Value.y) + (angle * dist);
                
                if (posToCheck.x > 127 || posToCheck.y > 127 || posToCheck.x <0 || posToCheck.y <0) break;
                
                int indexInLineOfSightGrid = (((int) posToCheck.y) * 128) + ((int) posToCheck.x);
                if (obstacleGrid[indexInLineOfSightGrid].present) 
                    break;
                lineOfSightGrid[indexInLineOfSightGrid] = new LineOfSightBufferElement {present = true};
            }
        }
    }
    
    public static void UpdateHomeLineInDirection(Translation start, DynamicBuffer<ObstacleBufferElement> obstacleGrid,
        DynamicBuffer<HomeLineOfSightBufferElement> lineOfSightGrid, int boardWidth)
    {
        int[] lineIndices = new int[boardWidth];
        for (int i = 0; i < 360; i++)
        {
            float2 angle = new float2( math.cos(i),  math.sin(i));
            for (int dist = 1; dist < boardWidth; dist++)
            {
                float2 posToCheck = new float2(start.Value.x, start.Value.y) + (angle * dist);
                
                if (posToCheck.x > 127 || posToCheck.y > 127 || posToCheck.x <0 || posToCheck.y <0) break;
                
                int indexInLineOfSightGrid = (((int) posToCheck.y) * 128) + ((int) posToCheck.x);
                if (obstacleGrid[indexInLineOfSightGrid].present) 
                    break;
                lineOfSightGrid[indexInLineOfSightGrid] = new HomeLineOfSightBufferElement {present = true};
            }
        }
    }
}




