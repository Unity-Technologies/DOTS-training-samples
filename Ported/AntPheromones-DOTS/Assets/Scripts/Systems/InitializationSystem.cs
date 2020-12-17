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
        
        var random = new Random(6541);
        var center = new Translation{ Value = new float3(64, 64, 0) };
        var minRange = new float2(-1,-1);
        var maxRange = new float2(1,1);
        var bottomLeftFood = new Translation {Value = new float3(10, 10, 0)};
        var topLeftFood = new Translation {Value = new float3(10, 118, 0)};
        var bottomRightFood = new Translation {Value = new float3(118, 10, 0)};
        var topRightFood = new Translation {Value = new float3(118, 118, 0)};
        
        Entity obstacleEntity = GetSingletonEntity<ObstacleBufferElement>();
        DynamicBuffer<ObstacleBufferElement> obstacleGrid = EntityManager.GetBuffer<ObstacleBufferElement>(obstacleEntity);
        
        
        Entities
            // .WithoutBurst()
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
                var randomFoodPosIndex = random.NextInt(0, 4);
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
                        int upRight = indexInObstacleGrid + boardWidth +1;
                        obstacleGrid[upRight] = new ObstacleBufferElement {present = true};
                        int upLeft = indexInObstacleGrid + boardWidth -1;
                        obstacleGrid[upLeft] = new ObstacleBufferElement {present = true};
                        int downRight = indexInObstacleGrid - boardWidth +1;
                        obstacleGrid[downRight] = new ObstacleBufferElement {present = true};
                        int downLeft = indexInObstacleGrid - boardWidth -1;
                        obstacleGrid[downLeft] = new ObstacleBufferElement {present = true};
                        int right = indexInObstacleGrid + 1;
                        obstacleGrid[right] = new ObstacleBufferElement {present = true};
                        int left = indexInObstacleGrid - 1;
                        obstacleGrid[left] = new ObstacleBufferElement {present = true};

                        ecb.SetComponent(obstacle, translation);
                        ecb.SetComponent(obstacle, new Radius{radius = 4});
                        
                    }
                    
                }
                
                // create a camera that sees only the obstacles, 
                // the output texture should match the grid
                

            }).Run();

        ecb.Playback(EntityManager);
        
        ecb.Dispose();
    }
}
