using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct TrainSpawner : IComponentData
{
    public Entity trainPrefab;
    public Entity commuterPrefab;
}


public class TrainSpawnerAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public GameObject trainPrefab;
    public GameObject commuterPrefab;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new TrainSpawner
        {
            trainPrefab = conversionSystem.GetPrimaryEntity(trainPrefab),
            commuterPrefab = conversionSystem.GetPrimaryEntity(commuterPrefab),
        });
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(trainPrefab);

        if(commuterPrefab != null)
            referencedPrefabs.Add(commuterPrefab);
    }
}