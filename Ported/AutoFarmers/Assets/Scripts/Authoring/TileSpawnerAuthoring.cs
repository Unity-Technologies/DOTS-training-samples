using Unity.Entities;
using UnityEngine;
using System.Collections.Generic;
using Unity.Mathematics;

public class TileSpawnerAuthoring : MonoBehaviour
    , IConvertGameObjectToEntity
    , IDeclareReferencedPrefabs
{
    public GameObject TilePrefab;
    public int2 GridSize;
    public float2 TileSize;
    public int Attempts;
    public GameObject RockPrefab;
    public int StoreCount;
    public GameObject SiloPrefab;

    // This function is required by IDeclareReferencedPrefabs
    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        // Conversion only converts the GameObjects in the scene.
        // This function allows us to inject extra GameObjects,
        // in this case prefabs that live in the assets folder.
        referencedPrefabs.Add(TilePrefab);
        referencedPrefabs.Add(RockPrefab);
        referencedPrefabs.Add(SiloPrefab);
    }

    // This function is required by IConvertGameObjectToEntity
    public void Convert(Entity entity, EntityManager dstManager
        , GameObjectConversionSystem conversionSystem)
    {

        var settingsEntity = conversionSystem.CreateAdditionalEntity(this);
        dstManager.AddComponentData(settingsEntity, new Settings
        {
            // ...
        });

        var tileBuffer = dstManager.AddBuffer<TileState>(settingsEntity);
        tileBuffer.Length = GridSize.x * GridSize.y;

        // GetPrimaryEntity fetches the entity that resulted from the conversion of
        // the given GameObject, but of course this GameObject needs to be part of
        // the conversion, that's why DeclareReferencedPrefabs is important here.
        dstManager.AddComponentData(entity, new TileSpawner
        {
            TilePrefab = conversionSystem.GetPrimaryEntity(TilePrefab),
            GridSize = GridSize,
            TileSize = TileSize,
            Attempts = Attempts,
            RockPrefab = conversionSystem.GetPrimaryEntity(RockPrefab),
            StoreCount = StoreCount,
            SiloPrefab = conversionSystem.GetPrimaryEntity(SiloPrefab),
            Settings = settingsEntity
        });
    }
}
