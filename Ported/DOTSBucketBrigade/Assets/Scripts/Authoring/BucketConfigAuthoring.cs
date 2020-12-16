using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class BucketConfigAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public GameObject m_BucketPrefab;
    
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity,
            new BucketSpawner
            {
                Prefab = conversionSystem.GetPrimaryEntity(m_BucketPrefab)
            });
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(m_BucketPrefab);
    }
}