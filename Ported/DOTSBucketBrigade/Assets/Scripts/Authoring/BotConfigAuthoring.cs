using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class BotConfigAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public GameObject m_ThrowerPrefab;
    public GameObject m_FetcherPrefab;
    public GameObject m_BotPrefab;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity,
            new BotSpawner
            {
                FetcherPrefab = conversionSystem.GetPrimaryEntity(m_FetcherPrefab),
                ThrowerPrefab = conversionSystem.GetPrimaryEntity(m_ThrowerPrefab),
                BotPrefab = conversionSystem.GetPrimaryEntity(m_BotPrefab)
            });
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(m_FetcherPrefab);
        referencedPrefabs.Add(m_ThrowerPrefab);
        referencedPrefabs.Add(m_BotPrefab);
    }
}
