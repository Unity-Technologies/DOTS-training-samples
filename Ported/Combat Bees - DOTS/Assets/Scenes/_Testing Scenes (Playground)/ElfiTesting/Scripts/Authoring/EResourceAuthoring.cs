using Combatbees.Testing.Elfi;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityGameObject = UnityEngine.GameObject;
using UnityRangeAttribute = UnityEngine.RangeAttribute;
using UnityMonoBehaviour = UnityEngine.MonoBehaviour;
using Random = Unity.Mathematics.Random;

namespace Combatbees.Testing.Elfi
{
    public class EResourceAuthoring : UnityMonoBehaviour
        , IConvertGameObjectToEntity
        , IDeclareReferencedPrefabs
    {
        public UnityGameObject ResourcePrefab;
        public float GridX;
        public float GridZ;
        public Random random;
        public int startResourceCount;
        public int spawnedResources;

        public void DeclareReferencedPrefabs(List<UnityGameObject> referencedPrefabs)
        {
            referencedPrefabs.Add(ResourcePrefab);
        }


        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {

            dstManager.AddComponentData(entity, new EResourceComponent()
            {
                gridX = GridX,
                gridZ = GridZ,
                resourcePrefab = conversionSystem.GetPrimaryEntity(ResourcePrefab),
                random = random,
                startResourceCount = startResourceCount,
                spawnedResources = spawnedResources

            });
        }
    }
}