using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using System.Collections.Generic;
using UnityEngine;

public partial struct PillSpawningSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        MazeConfig mazeConfig = SystemAPI.GetSingleton<MazeConfig>();
        PrefabConfig prefabConfig = SystemAPI.GetSingleton<PrefabConfig>();

        List<Vector2Int> spawnedLocations = new List<Vector2Int>();

        if(mazeConfig.PillsToSpawn >= mazeConfig.Width * mazeConfig.Height)
        {
            Debug.LogError("You're trying to spawn too many pills.... stop it");
            return;
        }

        for (int i = 0; i < mazeConfig.PillsToSpawn; ++i)
        {
            Entity pillEntity = state.EntityManager.Instantiate(prefabConfig.PillPrefab);
            TransformAspect transformAspect = SystemAPI.GetAspectRW<TransformAspect>(pillEntity);

            Vector2Int randomTile = mazeConfig.GetRandomTilePosition();
            while (spawnedLocations.Contains(randomTile))
            {
                randomTile = mazeConfig.GetRandomTilePosition();
            }

            spawnedLocations.Add(randomTile);
            transformAspect.Position = new float3(randomTile.x, transformAspect.Position.y, randomTile.y);
        }

        state.Enabled = false;
    }
}
