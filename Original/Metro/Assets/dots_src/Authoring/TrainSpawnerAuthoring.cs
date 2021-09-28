using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class TrainSpawnerAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public GameObject TrainPrefab;
    public GameObject CarriagePrefab;
    
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData<TrainSpawner>(entity, new TrainSpawner
        {
            CarriagePrefab = conversionSystem.GetPrimaryEntity(CarriagePrefab),
            TrainPrefab = conversionSystem.GetPrimaryEntity(TrainPrefab)
        });
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(CarriagePrefab);
        referencedPrefabs.Add(TrainPrefab);
    }
}
