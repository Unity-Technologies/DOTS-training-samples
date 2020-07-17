using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct TrainSpawner : IComponentData
{
    public int numberOfTrainsPerTrack;
    public Entity trainPrefab;
}


public class TrainSpawnerAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public int numberOfTrainsPerTrack;
    public GameObject trainPrefab;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new TrainSpawner
        {
            numberOfTrainsPerTrack = numberOfTrainsPerTrack,
            trainPrefab = conversionSystem.GetPrimaryEntity(trainPrefab),
        });
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(trainPrefab);
    }
}