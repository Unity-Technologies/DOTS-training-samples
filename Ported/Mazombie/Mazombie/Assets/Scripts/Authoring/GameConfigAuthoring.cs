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
    public int mazeSize;
    public int openStripWidth;
    public int mazeStripWidth;


    public float cellSize = 1.0f;
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
            mazeSize = authoring.mazeSize,
            openStripWidth = authoring.openStripWidth,
            mazeStripWidth = authoring.mazeStripWidth
        });
    }
}