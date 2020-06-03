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

    
    public GameObject ScooperPrefab;
    public GameObject ThrowerPrefab;    
    public GameObject PasserForwardPrefab;
    public GameObject PasserBackPrefab;
    
    // Referenced prefabs have to be declared so that the conversion system knows about them ahead of time
    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(BucketPrefab);
        referencedPrefabs.Add(FireCellPrefab);
        referencedPrefabs.Add(ScooperPrefab);
        referencedPrefabs.Add(ThrowerPrefab);
        referencedPrefabs.Add(PasserForwardPrefab);
        referencedPrefabs.Add(PasserBackPrefab);
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
            ScooperPrefab = conversionSystem.GetPrimaryEntity(ScooperPrefab),
            ThrowerPrefab = conversionSystem.GetPrimaryEntity(ThrowerPrefab),
            PasserForwardPrefab = conversionSystem.GetPrimaryEntity(PasserForwardPrefab),
            PasserBackPrefab = conversionSystem.GetPrimaryEntity(PasserBackPrefab)
        };
        dstManager.AddComponentData(entity, spawnerData);
    }
}

public struct SpawnerConfig : IComponentData
{
    public Entity BucketPrefab;
    public Entity FireCellPrefab;
    public Entity ScooperPrefab;
    public Entity ThrowerPrefab;    
    public Entity PasserForwardPrefab;
    public Entity PasserBackPrefab;
}
