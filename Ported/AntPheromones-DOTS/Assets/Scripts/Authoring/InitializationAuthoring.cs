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
    public GameObject boardPrefab;
    
    public void Convert(Entity entity, EntityManager entityManager, GameObjectConversionSystem conversionSystem)
    {
        entityManager.AddComponentData(entity, new Init()
        {
            antCount = antCount,
            antPrefab = conversionSystem.GetPrimaryEntity(antPrefab),
            obstaclePrefab = conversionSystem.GetPrimaryEntity(obstaclePrefab),
            goalPrefab = conversionSystem.GetPrimaryEntity(goalPrefab),
            homePrefab = conversionSystem.GetPrimaryEntity(homePrefab),
            boardPrefab = conversionSystem.GetPrimaryEntity(boardPrefab)
        });
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(antPrefab);
        referencedPrefabs.Add(obstaclePrefab);
        referencedPrefabs.Add(goalPrefab);
        referencedPrefabs.Add(homePrefab);
        referencedPrefabs.Add(boardPrefab);
    }
}
