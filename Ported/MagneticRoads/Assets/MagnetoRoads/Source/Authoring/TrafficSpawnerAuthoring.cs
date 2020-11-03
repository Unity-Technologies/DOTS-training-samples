using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class TrafficSpawnerAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    , IDeclareReferencedPrefabs
{
    public GameObject CarPrefab;
    public GameObject SimpleIntersectionPrefab;
    public GameObject DoubleIntersectionPrefab;
    public GameObject TripleIntersectionPrefab;

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(CarPrefab);
        referencedPrefabs.Add(SimpleIntersectionPrefab);
        referencedPrefabs.Add(DoubleIntersectionPrefab);
        referencedPrefabs.Add(TripleIntersectionPrefab);
    }

    public void Convert(Entity entity, EntityManager dstManager
        , GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new TrafficSpawner
        {
            CarPrefab = conversionSystem.GetPrimaryEntity(CarPrefab),
            SimpleIntersectionPrefab = conversionSystem.GetPrimaryEntity(SimpleIntersectionPrefab),
            DoubleIntersectionPrefab = conversionSystem.GetPrimaryEntity(DoubleIntersectionPrefab),
            TripleIntersectionPrefab = conversionSystem.GetPrimaryEntity(TripleIntersectionPrefab)
        });
    }
}
