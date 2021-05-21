using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class BeeSpawnerAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public GameObject BeePrefab;
    
    [Range(0, 10000)] public int BeeCount;
    [Range(0, 10000)] public int BeeCountFromResource = 10;

    [Range(0, 20)] public int MinSpeed = 1;
    [Range(0, 100)] public int MaxSpeed = 20;

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(BeePrefab);
    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new BeeSpawner
        {
            BeePrefab = conversionSystem.GetPrimaryEntity(BeePrefab),
            BeeCount = BeeCount,
            BeeCountFromResource = BeeCountFromResource,
            MinSpeed = MinSpeed,
            MaxSpeed = MaxSpeed
        });
    }
}
