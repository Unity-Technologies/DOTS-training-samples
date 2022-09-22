using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;

[BurstCompile]
public partial struct PillSpawningSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PrefabConfig>();
        state.RequireForUpdate<MazeConfig>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        MazeConfig mazeConfig = SystemAPI.GetSingleton<MazeConfig>();
        if (mazeConfig.SpawnPills)
        {
            Entity mazeConfigEntity = SystemAPI.GetSingletonEntity<MazeConfig>();
            mazeConfig.SpawnPills = false;
            SystemAPI.SetComponent<MazeConfig>(mazeConfigEntity, mazeConfig);

            PrefabConfig prefabConfig = SystemAPI.GetSingleton<PrefabConfig>();

            NativeArray<int2> spawnedLocations = CollectionHelper.CreateNativeArray<int2>(mazeConfig.PillsToSpawn, Allocator.Temp);

            if (mazeConfig.PillsToSpawn >= mazeConfig.Width * mazeConfig.Height)
            {
                Debug.LogError("You're trying to spawn too many pills.... stop it");
                return;
            }

            for (int i = 0; i < mazeConfig.PillsToSpawn; ++i)
            {
                Entity pillEntity = state.EntityManager.Instantiate(prefabConfig.PillPrefab);
                TransformAspect transformAspect = SystemAPI.GetAspectRW<TransformAspect>(pillEntity);

                int2 randomTile = mazeConfig.GetRandomTilePosition();
                while (spawnedLocations.Contains(randomTile))
                {
                    randomTile = mazeConfig.GetRandomTilePosition();
                }

                spawnedLocations[i] = randomTile;
                transformAspect.Position = new float3(randomTile.x, transformAspect.Position.y, randomTile.y);
            }

        }
    }
}
