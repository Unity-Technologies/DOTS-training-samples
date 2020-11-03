using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[DisallowMultipleComponent]
public class MazeAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public int2 tileMazeSize = new int2(70,70);

    [Header("Prefabs")]
    public GameObject tilePrefab;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var tilePrefabEntity = conversionSystem.GetPrimaryEntity(tilePrefab);
        var spawner = new TileSpawner
        {
            TileSize =  tileMazeSize,
            TilePrefab = tilePrefabEntity
        };

        dstManager.AddComponentData(entity, spawner);
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(tilePrefab);
    }
}
