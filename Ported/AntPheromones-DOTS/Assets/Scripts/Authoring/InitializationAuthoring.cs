using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class InitializationAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public GameObject antPrefab;
    public int antCount;
    public GameObject obstaclePrefab;
    public GameObject goalPrefab;
    public GameObject homePrefab;
    
    public void Convert(Entity entity, EntityManager entityManager, GameObjectConversionSystem conversionSystem)
    {
        entityManager.AddComponentData(entity, new Init()
        {
            antPrefab = conversionSystem.GetPrimaryEntity(antPrefab),
            antCount = antCount,
            obstaclePrefab = conversionSystem.GetPrimaryEntity(obstaclePrefab),
            goalPrefab = conversionSystem.GetPrimaryEntity(goalPrefab),
            homePrefab = conversionSystem.GetPrimaryEntity(homePrefab)
        });
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(antPrefab);
        referencedPrefabs.Add(obstaclePrefab);
        referencedPrefabs.Add(goalPrefab);
        referencedPrefabs.Add(homePrefab);
    }
}
