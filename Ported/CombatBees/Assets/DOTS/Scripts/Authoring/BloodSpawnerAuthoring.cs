using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class BloodSpawnerAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public GameObject BloodPrefab;

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(BloodPrefab);
    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new Blood
        {
            BloodPrefab = conversionSystem.GetPrimaryEntity(BloodPrefab)
        });
    }
}
