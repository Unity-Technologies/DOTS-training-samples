using System.Collections.Generic;
using Unity.Entities;
using UnityGameObject = UnityEngine.GameObject;
using UnityRangeAttribute = UnityEngine.RangeAttribute;
using UnityMonoBehaviour = UnityEngine.MonoBehaviour;

namespace Authoring
{
    public class SpawnerAuthoring : UnityMonoBehaviour
        , IConvertGameObjectToEntity
        , IDeclareReferencedPrefabs
    {
        public UnityGameObject BeePrefab;
        [UnityRange(0, 1000)] public int BeesAmount;

        public UnityGameObject ResourcePrefab;
        [UnityRange(0, 1000)] public int ResourceAmount;

        public void DeclareReferencedPrefabs(List<UnityGameObject> referencedPrefabs)
        {
            referencedPrefabs.Add(BeePrefab);
            referencedPrefabs.Add(ResourcePrefab);
        }

        public void Convert(Entity entity, EntityManager dstManager
            , GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new HiveSpawner()
                {
                    BeePrefab = conversionSystem.GetPrimaryEntity(BeePrefab),
                    BeesAmount = BeesAmount,
                    ResourcePrefab = conversionSystem.GetPrimaryEntity(ResourcePrefab),
                    ResourceAmount = ResourceAmount
                }
            );
        }
    }
}