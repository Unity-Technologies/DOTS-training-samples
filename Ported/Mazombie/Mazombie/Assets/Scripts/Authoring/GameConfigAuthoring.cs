using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class GameConfigAuthoring : MonoBehaviour
{
    public GameObject tileGO;
    public GameObject wallPrefab;
    public GameObject playerSpawnPrefab;
    public GameObject playerPrefab;
    public GameObject zombiePrefab;
    
    public int mazeSize;

    public float cellSize = 1.0f;
    public int num_zombies = 10;
}

public class GameConfigBaker : Baker<GameConfigAuthoring>
{
    public override void Bake(GameConfigAuthoring authoring)
    {
        AddComponent(new GameConfig
        {
            tile = GetEntity(authoring.tileGO),
            wallPrefab = GetEntity(authoring.wallPrefab),
            playerSpawnPrefab = GetEntity(authoring.playerSpawnPrefab),
            mazeSize = authoring.mazeSize,
            num_zombies = authoring.num_zombies,
            zombiePrefab = GetEntity(authoring.zombiePrefab)
        });
    }
}