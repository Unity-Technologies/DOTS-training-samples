using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class WaterConfigAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public GameObject m_WaterPrefab;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity,
            new WaterSpawner
            {
                Prefab = conversionSystem.GetPrimaryEntity(m_WaterPrefab)
            });
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(m_WaterPrefab);
    }
}
