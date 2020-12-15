using Unity.Entities;
using UnityEngine;
using System.Collections.Generic;
using Unity.Mathematics;

public class TileSpawnerAuthoring : MonoBehaviour
    , IConvertGameObjectToEntity
    , IDeclareReferencedPrefabs
{
    public GameObject TilePrefab;
    // [Range(0, 1000)] public int LaneCount;
    public int2 GridSize; 

    // This function is required by IDeclareReferencedPrefabs
    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        // Conversion only converts the GameObjects in the scene.
        // This function allows us to inject extra GameObjects,
        // in this case prefabs that live in the assets folder.
        referencedPrefabs.Add(TilePrefab);
    }

    // This function is required by IConvertGameObjectToEntity
    public void Convert(Entity entity, EntityManager dstManager
        , GameObjectConversionSystem conversionSystem)
    {
        // GetPrimaryEntity fetches the entity that resulted from the conversion of
        // the given GameObject, but of course this GameObject needs to be part of
        // the conversion, that's why DeclareReferencedPrefabs is important here.
        dstManager.AddComponentData(entity, new TileSpawner
        {
            TilePrefab = conversionSystem.GetPrimaryEntity(TilePrefab),
            GridSize = GridSize,
        });
    }
}