using System.Collections.Generic;
using Unity.Entities;
using Unity.Rendering;
using UnityEngine;

public class SpawnerAuthoring : MonoBehaviour
    , IConvertGameObjectToEntity
    , IDeclareReferencedPrefabs
{
    public GameObject ScooperPrefab;
    public GameObject BucketPrefab;
    public GameObject FireCell;
    public GameObject WaterCell;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new Spawner
        {
            ScooperPrefab = conversionSystem.GetPrimaryEntity(ScooperPrefab),
            BucketPrefab = conversionSystem.GetPrimaryEntity(BucketPrefab),
            FireCell = conversionSystem.GetPrimaryEntity(FireCell),
            WaterCell = conversionSystem.GetPrimaryEntity(WaterCell),
        });
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(ScooperPrefab);
        referencedPrefabs.Add(BucketPrefab);
        referencedPrefabs.Add(FireCell);
        referencedPrefabs.Add(WaterCell);
    }
}


