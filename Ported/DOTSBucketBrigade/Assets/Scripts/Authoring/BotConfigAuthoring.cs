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
            new FetcherSpawner
            {
                Prefab = conversionSystem.GetPrimaryEntity(m_FetcherPrefab)
            });
        dstManager.AddComponentData(entity,
            new ThrowerSpawner
            {
                Prefab = conversionSystem.GetPrimaryEntity(m_ThrowerPrefab)
            });
        dstManager.AddComponentData(entity,
            new BucketTeamSpawner
            {
                Prefab = conversionSystem.GetPrimaryEntity(m_BotPrefab)
            });
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(m_FetcherPrefab);
        referencedPrefabs.Add(m_ThrowerPrefab);
        referencedPrefabs.Add(m_BotPrefab);
    }
}
