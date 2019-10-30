using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public struct BarSpawner : IComponentData
{
    public Entity prefab;
    public int pointCountBuildings;
    public int pointCountDetails;
}

public class BarSpawnerAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public GameObject prefab;
    public int pointCountBuildings;
    public int pointCountDetails;
    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(prefab);
    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        BarSpawner spawner;
        spawner.prefab = conversionSystem.GetPrimaryEntity(prefab);
        spawner.pointCountBuildings = pointCountBuildings;
        spawner.pointCountDetails = pointCountDetails;
        dstManager.AddComponentData<BarSpawner>(entity, spawner);
    }
}
