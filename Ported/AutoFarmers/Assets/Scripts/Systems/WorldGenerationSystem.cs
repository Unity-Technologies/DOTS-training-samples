using Unity.Entities;
using Unity.Burst;
using Unity.Transforms;
using System.Diagnostics;
using Unity.Rendering;
using Unity.Mathematics;
using System;
using Random = Unity.Mathematics.Random;
using UnityEngine;
using static UnityEditor.PlayerSettings;
using Unity.Collections;
using UnityEditor.PackageManager;

[BurstCompile]
public partial struct WorldGenerationSystem : ISystem
{
    Random random;
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        random = Random.CreateFromIndex(1234);
        state.RequireForUpdate<WorldGrid>();
        state.RequireForUpdate<Config>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<Config>();
        var worldGrid = SystemAPI.GetSingleton<WorldGrid>();
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
        

        //worldGrid.typeGrid = new NativeArray<byte>(worldGrid.gridSize.x * worldGrid.gridSize.y, Allocator.Persistent);
        //worldGrid.entityGrid = new NativeArray<Entity>(worldGrid.gridSize.x * worldGrid.gridSize.y, Allocator.Persistent);

        int width = worldGrid.gridSize.x;
        int height = worldGrid.gridSize.y;
        float rockNoiseThreshold = 0.2f;
        float safeZone = config.safeZoneRadius * config.safeZoneRadius;

        float maxNoiseVal = -math.INFINITY;
        float minNoiseVal = math.INFINITY;

        float2 siloNoiseThreshold = new float2(-0.6f, -0.605f);

        //Let's make the chunks first
        DynamicBuffer<ChunkCell> chunkBuffer = state.EntityManager.GetBuffer<ChunkCell>(worldGrid.entity);

        chunkBuffer.Length = (width * height) / ChunkCell.size;



        //foreach(var cell in chunkBuffer)
        //{
        //    cell.typeCount = new NativeArray<int>();
        //}

        for(int x = 0;x< width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float3 gridWorldPos = worldGrid.GridToWorld(x, y);
                if (math.lengthsq(gridWorldPos) < safeZone) continue;

                var pos = new float2(x, y);
                float noiseVal = noise.cnoise(pos / 10f);

                if (noiseVal < minNoiseVal) minNoiseVal = noiseVal;
                if (noiseVal > maxNoiseVal) maxNoiseVal = noiseVal;

                float remappedNoise = math.remap(rockNoiseThreshold, 1.0f, 20, 100, noiseVal);

                if (noiseVal > rockNoiseThreshold)
                {
                    worldGrid.SetTypeAt(x, y,Rock.type);
                    //Create it
                    Entity rock = state.EntityManager.Instantiate(config.RockPrefab);
                    RockAspect rAspect = SystemAPI.GetAspectRW<RockAspect>(rock);
                    rAspect.Health = (int)remappedNoise;
                    rAspect.Transform.LocalPosition = gridWorldPos;
                    worldGrid.SetEntityAt(x,y,rock);
                }

                if (noiseVal < siloNoiseThreshold.x && noiseVal > siloNoiseThreshold.y)
                {
                    worldGrid.SetTypeAt(x, y, Silo.type);
                    //Create it
                    Entity silo = state.EntityManager.Instantiate(config.SiloPrefab);
                    SiloAspect rAspect = SystemAPI.GetAspectRW<SiloAspect>(silo);
                    //rAspect.Health = (int)remappedNoise;
                    rAspect.Transform.LocalPosition = gridWorldPos;
                    worldGrid.SetEntityAt(x, y, silo);
                }

                //if(noiseVal)
            }
        }

        


        state.Enabled = false;

    }

}

