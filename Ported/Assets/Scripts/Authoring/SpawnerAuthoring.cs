using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Serialization;

public class SpawnerAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public GameObject antPrefab;
    public GameObject colonyPrefab;
    public GameObject groundPrefab;
    public GameObject obstaclePrefab;
    public GameObject resourcePrefab;

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(antPrefab);
        referencedPrefabs.Add(groundPrefab);
        referencedPrefabs.Add(colonyPrefab);
        referencedPrefabs.Add(obstaclePrefab);
        referencedPrefabs.Add(resourcePrefab);
    }
    
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new Spawner()
        {
            AntPrefab = conversionSystem.GetPrimaryEntity(antPrefab),
            ColonyPrefab = conversionSystem.GetPrimaryEntity(colonyPrefab),
            GroundPrefab = conversionSystem.GetPrimaryEntity(groundPrefab),
            ObstaclePrefab = conversionSystem.GetPrimaryEntity(obstaclePrefab),
            ResourcePrefab = conversionSystem.GetPrimaryEntity(resourcePrefab),
        });
    }
}
