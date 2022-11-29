using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;
using Unity.Transforms;
using Random = Unity.Mathematics.Random;

partial struct WorldSpawnerSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        hasSpawnedWalls = false;
    }

    public void OnDestroy(ref SystemState state)
    {

    }

    private bool hasSpawnedWalls;
    public void OnUpdate(ref SystemState state)
    {
        if(UnityEngine.Input.GetKeyDown(KeyCode.Space) || !hasSpawnedWalls)
            SpawnWalls(ref state);

        foreach (var wall in SystemAPI.Query<TransformAspect, WorldTransform>())
        {
            wall.Item1.WorldPosition = wall.Item2._Position;
            wall.Item1.WorldScale = wall.Item2.Scale;
        }
        
    }
    
    void SpawnWalls(ref SystemState state)
    {
        hasSpawnedWalls = true;
        
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        ConfigAspect config = new ConfigAspect();

        foreach (var c in SystemAPI.Query<ConfigAspect>())
        {
            config = c;
        }

        float currentAngle = 0;
        float angleBetweenWalls = 0;
        

        float currentDistance = 0;
        
        int WallRingCount = config.WallCount;
        float MinWallGap = 25.0f;
        float MaxWallGap = 80.0f;
        float WallSpacing = config.PlaySize / (WallRingCount + 3);
        float distanceBetweenWalls = 0.5f;

        int MinGapCount = 1;
        int MaxGapCount = 3;
        var random = Random.CreateFromIndex(12);

        NativeList<float> Gaps = new NativeList<float>(Allocator.Temp);
        for (int i = 1; i <= WallRingCount; ++i)
        {
            currentDistance += WallSpacing;
            angleBetweenWalls = distanceBetweenWalls / currentDistance;
            
            
            var gapCount = random.NextInt(MinGapCount, MaxGapCount);

            int createdGaps = 0;
            while (createdGaps < gapCount)
            {
                var startPos = random.NextFloat(0, math.PI * 2.0f);
              
                var endPos = startPos + math.radians(random.NextFloat(MinWallGap, MaxWallGap));

                if (endPos > math.PI * 2.0f)
                {
                    Gaps.Add(startPos);
                    Gaps.Add(math.PI * 2.0f);
                    
                    Gaps.Add(0);
                    Gaps.Add(endPos - math.PI * 2.0f);
                }
                else
                {
                    Gaps.Add(startPos);
                    Gaps.Add(endPos);
                }
                
                ++createdGaps;
            }

            while (currentAngle < math.PI*2.0f)
            {
                var pos = new float2();

                float angleSkip = 0;
                for (int s = 0; s + 1 < Gaps.Length; s += 2)
                {
                    if (currentAngle > Gaps[s] && currentAngle < Gaps[s + 1])
                    {
                        angleSkip = Gaps[s + 1] - currentAngle;
                    }
                }

                pos.x = math.cos(currentAngle) * currentDistance;
                pos.y = math.sin(currentAngle) * currentDistance;
                
                var wall = ecb.Instantiate(config.WallPrefab);
                
                var trans = new WorldTransform{_Position = new float3(pos.x, 0, pos.y), _Scale = 1.0f};
                ecb.SetComponent(wall , trans);
                ecb.SetComponent(wall, new Obstacle());
                currentAngle += angleBetweenWalls + angleSkip;
            }

            currentAngle = 0;
            Gaps.Clear();
        }

        //spawn colony and food
        var food = ecb.Instantiate(config.FoodPrefab);
        float spawnAngle = random.NextFloat(0, math.PI * 2.0f);
        float3 spawnLocation = new float3(math.cos(spawnAngle), 0, math.sin(spawnAngle));
        spawnLocation *= (WallSpacing * 4.0f);
        var transform = new WorldTransform { _Position = spawnLocation, _Scale = 1.0f };
        ecb.SetComponent(food, transform);
        ecb.SetComponent(food, new Food());

        var colony = ecb.Instantiate(config.ColonyPrefab);
        transform = new WorldTransform { _Position = new float3(0, 0, 0), _Scale = 1.0f };
        ecb.SetComponent(colony, transform);
        ecb.SetComponent(colony, new Colony());

    }
}
