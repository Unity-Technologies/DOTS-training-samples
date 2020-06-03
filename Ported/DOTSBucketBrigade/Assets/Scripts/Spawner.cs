using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

// ReSharper disable once InconsistentNaming
[RequiresEntityConversion]
[ConverterVersion("joe", 1)]
public class Spawner : MonoBehaviour, IDeclareReferencedPrefabs, IConvertGameObjectToEntity
{
    public GameObject BucketPrefab;
    public GameObject FireCellPrefab;

    // Referenced prefabs have to be declared so that the conversion system knows about them ahead of time
    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(BucketPrefab);
        referencedPrefabs.Add(FireCellPrefab);
    }

    // Lets you convert the editor data representation to the entity optimal runtime representation
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var spawnerData = new SpawnerConfig
        {
            // The referenced prefab will be converted due to DeclareReferencedPrefabs.
            // So here we simply map the game object to an entity reference to that prefab.
            BucketPrefab = conversionSystem.GetPrimaryEntity(BucketPrefab),
            FireCellPrefab = conversionSystem.GetPrimaryEntity(FireCellPrefab),
        };
        dstManager.AddComponentData(entity, spawnerData);
    }
}

public struct SpawnerConfig : IComponentData
{
    public Entity BucketPrefab;
    public Entity FireCellPrefab;
}