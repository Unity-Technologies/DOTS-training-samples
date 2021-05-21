using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class ResourceSpawnerAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public GameObject ResourcePrefab;
    
    [Range(0, 10000)] public int ResourceCount;

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(ResourcePrefab);
    }
    
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new ResourceSpawner
        {
            ResourcePrefab = conversionSystem.GetPrimaryEntity(ResourcePrefab),
            ResourceCount = ResourceCount,
        });
    }
}
