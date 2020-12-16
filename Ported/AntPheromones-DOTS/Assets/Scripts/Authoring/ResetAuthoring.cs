using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class ResetAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public GameObject obstaclePrefab;
    
    public void Convert(Entity entity, EntityManager entityManager, GameObjectConversionSystem conversionSystem)
    {
        entityManager.AddComponentData(entity, new Reset()
        {
            obstaclePrefab = conversionSystem.GetPrimaryEntity(obstaclePrefab),
        });
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(obstaclePrefab);
    }
}
