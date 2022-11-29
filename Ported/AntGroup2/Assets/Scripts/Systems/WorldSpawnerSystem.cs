using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
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

        foreach (var wall in SystemAPI.Query<TransformAspect, WorldTransform>().WithAll<Obstacle>())
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
        float MinWallGap = 15.0f;
        float MaxWallGap = 40.0f;
        float WallSpacing = config.PlaySize / (WallRingCount + 3);
        float distanceBetweenWalls = 0.5f;

        //int MinGapCount = 1;
        //int MaxGapCount = 3;
        //var random = Random.CreateFromIndex(123456);
//
        //NativeList<float> Gaps = new NativeList<float>();
//
        for (int i = 1; i <= WallRingCount; ++i)
        {
            currentDistance += WallSpacing;
            angleBetweenWalls = distanceBetweenWalls / currentDistance;
            
            
            //var gapCount = random.NextInt(MinGapCount, MaxGapCount);

            int createdGaps = 0;
            //while (createdGaps < gapCount)
            //{
            //    var startPos = random.NextFloat(0, math.PI * 2.0f);
            //    Gaps.Add(startPos);
            //    var endPos = startPos + math.radians(random.NextFloat(MinWallGap, MaxWallGap));
            //    Gaps.Add(endPos);
            //    ++createdGaps;
            //}

            while (currentAngle < math.PI*2.0f)
            {
                var pos = new float2();

                if (currentAngle > 1.0f && currentAngle < 1.4f)
                {
                    currentAngle += 0.4f;
                }

                pos.x = math.cos(currentAngle) * currentDistance;
                pos.y = math.sin(currentAngle) * currentDistance;
                
                var wall = ecb.Instantiate(config.WallPrefab);
                
                var trans = new WorldTransform{_Position = new float3(pos.x, 0, pos.y), _Scale = 1.0f};
                ecb.SetComponent(wall , trans);
                ecb.SetComponent(wall, new Obstacle());
                currentAngle += angleBetweenWalls;
            }

            currentAngle = 0;
        }


        
    }
}
