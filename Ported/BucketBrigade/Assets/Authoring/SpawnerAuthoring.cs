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
    [Range(0,100)] public int BucketCount;
    public GameObject FireCell;
    public int FireGridDimension;
    public GameObject WaterCell;
    public int WaterCellCount;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new Spawner
        {
            ScooperPrefab = conversionSystem.GetPrimaryEntity(ScooperPrefab),
            BucketPrefab = conversionSystem.GetPrimaryEntity(BucketPrefab),
            BucketCount = BucketCount,
            FireCell = conversionSystem.GetPrimaryEntity(FireCell),
            FireGridDimension = FireGridDimension,
            WaterCell = conversionSystem.GetPrimaryEntity(WaterCell),
            WaterCellCount = WaterCellCount
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


