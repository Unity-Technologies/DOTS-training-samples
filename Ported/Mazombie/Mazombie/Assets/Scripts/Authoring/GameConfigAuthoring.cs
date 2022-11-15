using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class GameConfigAuthoring : MonoBehaviour
{
    public GameObject tileGO;
    public GameObject wallPrefab;

    public int mazeSize;

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
            mazeSize = authoring.mazeSize,
        });
    }
}