using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;

public class FireSimConfigAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public GameObject m_FirePrefab;
    public GameObject m_ThrowerPrefab;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity,
                    new FireCellSpawner
                    {
                        Prefab = conversionSystem.GetPrimaryEntity(m_FirePrefab)
                    });

        dstManager.AddComponentData<ThrowerSpawner>(entity, new ThrowerSpawner
        {
            Prefab = conversionSystem.GetPrimaryEntity(m_ThrowerPrefab)
        });
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(m_FirePrefab);
        referencedPrefabs.Add(m_ThrowerPrefab);
    }
}
