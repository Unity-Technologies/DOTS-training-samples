using System.Collections.Generic;
using src.DOTS.Components;
using Unity.Entities;
using UnityEngine;

namespace src.DOTS.Authoring
{
    public class CommuterSpawnerAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
    {
        public GameObject m_CommuterPrefab;
        public int amountToSpawn;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new CommuterSpawner
            {
                commuterPrefab = conversionSystem.GetPrimaryEntity(m_CommuterPrefab),
                amountToSpawn = amountToSpawn
            });
        }

        public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
        {
            referencedPrefabs.Add(m_CommuterPrefab);
        }
    }
}