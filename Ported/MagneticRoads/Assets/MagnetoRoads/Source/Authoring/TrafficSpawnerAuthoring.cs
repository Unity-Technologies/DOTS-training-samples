using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class TrafficSpawnerAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    , IDeclareReferencedPrefabs
{
    public GameObject CarPrefab;

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(CarPrefab);
    }

    public void Convert(Entity entity, EntityManager dstManager
        , GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new TrafficSpawner
        {
            CarPrefab = conversionSystem.GetPrimaryEntity(CarPrefab)
        });
    }
}
