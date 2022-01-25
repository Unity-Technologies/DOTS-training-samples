using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class SpawnerAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public GameObject TilePrefab;
    [Range(2, 200)]
    public int TileMapWidth;
    [Range(2, 200)]
    public int TileMapHeight;

    public Color TileOddColor;
    public Color TileEvenColor;

    [Range(0, 0.5f)]
    public float WallFrequency;

    public uint MapSeed = 1234;
    
    public GameObject WallPrefab;
    public GameObject CatPrefab;
    public GameObject MousePrefab;
    public GameObject ArrowPrefab;
    public GameObject ExitPrefab;
    
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new MapSpawner
        {
            TilePrefab = conversionSystem.GetPrimaryEntity(TilePrefab),
            MapWidth = TileMapWidth,
            MapHeight = TileMapHeight,
            MapSeed = MapSeed,
            WallPrefab = conversionSystem.GetPrimaryEntity(WallPrefab),
            WallFrequency = WallFrequency,
            TileEvenColor = new float4(TileEvenColor.r, TileEvenColor.g, TileEvenColor.b, TileEvenColor.a),
            TileOddColor = new float4(TileOddColor.r, TileOddColor.g, TileOddColor.b, TileOddColor.a)
        });
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(TilePrefab);
        referencedPrefabs.Add(WallPrefab);
        referencedPrefabs.Add(CatPrefab);
        referencedPrefabs.Add(MousePrefab);
        referencedPrefabs.Add(ArrowPrefab);
        referencedPrefabs.Add(ExitPrefab);
    }
}
