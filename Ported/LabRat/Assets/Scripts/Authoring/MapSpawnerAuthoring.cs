using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class MapSpawnerAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
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

    public GameObject WallPrefab;
    
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new MapSpawner
        {
            TilePrefab = conversionSystem.GetPrimaryEntity(TilePrefab),
            MapWidth = TileMapWidth,
            MapHeight = TileMapHeight,
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
    }
}
