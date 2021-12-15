using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityGameObject = UnityEngine.GameObject;
using UnityRangeAttribute = UnityEngine.RangeAttribute;
using UnityMonoBehaviour = UnityEngine.MonoBehaviour;

namespace Combatbees.Testing.Mahmoud
{
    public class ResourceAuthoring : UnityMonoBehaviour
        , IConvertGameObjectToEntity
        , IDeclareReferencedPrefabs
    {
        public UnityGameObject ResourcePrefab;
        public int GridX;
        public int GridY;

        public void DeclareReferencedPrefabs(List<UnityGameObject> referencedPrefabs)
        {
            referencedPrefabs.Add(ResourcePrefab);
        }


        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {

            dstManager.AddComponentData(entity, new ResourceComponent()
            {
                gridX = GridX,
                gridY = GridY,
                resourcePrefab = conversionSystem.GetPrimaryEntity(ResourcePrefab)

            });
        }
    }
}