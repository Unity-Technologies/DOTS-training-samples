using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityGameObject = UnityEngine.GameObject;
using UnityRangeAttribute = UnityEngine.RangeAttribute;
using UnityMonoBehaviour = UnityEngine.MonoBehaviour;

namespace Dots
{
    [DisallowMultipleComponent]
    public class BuildingSpawnerAuthoring : UnityMonoBehaviour
        , IConvertGameObjectToEntity
        , IDeclareReferencedPrefabs
    {
        public UnityGameObject BeamPrefab;
        [UnityRange(1, 100)]
        public int BuildingCount = 35;

        public bool UseBeamGroups = true;
        
        // This function is required by IDeclareReferencedPrefabs
        public void DeclareReferencedPrefabs(List<UnityGameObject> referencedPrefabs)
        {
            referencedPrefabs.Add(BeamPrefab);
        }
        
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new Dots.BuildingSpawnerData
            {
                BeamPrefab = conversionSystem.GetPrimaryEntity(BeamPrefab),
                BuildingCount = BuildingCount,
                UseBeamGroups = UseBeamGroups
            });
        }
    }
}
