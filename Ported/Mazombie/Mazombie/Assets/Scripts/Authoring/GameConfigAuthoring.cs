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
    public GameObject pillPrefab;

    public int mazeSize;
    public bool parallelMazeGenAlgorithm = false;
    public int openStripCount;
    public int mazeStripWidth;
    public GameObject movingWallPrefab;
    public int numMovingWalls;
    public int movingWallsLength;
    public int movingWallRangeMin;
    public int movingWallRangeMax;
    public float cellSize = 1.0f;
    public int num_zombies = 10;
    public int numPills = 10;
    public uint seed = 42;
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
            playerPrefab = GetEntity(authoring.playerPrefab),
            movingWallPrefab = GetEntity(authoring.movingWallPrefab),
            pillPrefab = GetEntity(authoring.pillPrefab),
            numMovingWalls = authoring.numMovingWalls,
            movingWallsLength = authoring.movingWallsLength,
            movingWallRangeMin = authoring.movingWallRangeMin,
            movingWallRangeMax = authoring.movingWallRangeMax,
            mazeSize = authoring.mazeSize,
            parallelMazeGen = authoring.parallelMazeGenAlgorithm,
            openStripCount = authoring.openStripCount,
            mazeStripWidth = authoring.mazeStripWidth,
            num_zombies = authoring.num_zombies,
            numPills = authoring.numPills,
            zombiePrefab = GetEntity(authoring.zombiePrefab),
            seed = authoring.seed,
            gameState = { score = 0 },
        });
    }
}