using System.Collections.Generic;
using Unity.Entities;
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
        public UnityGameObject beamPrefab;
        [UnityRange(1, 500)] public int buildingCount = 35;
        [UnityRange(10, 500)] public int buildingAreaSize = 140;
        [UnityRange(4, 100)] public int buildingMaxFloorCount = 12;
        
        [UnityRange(10, 8000)] public int groundBeamsPointCount = 600;
        [UnityRange(10, 500)] public int groundBeamsAreaSize = 120;
        [UnityRange(1, 10)] public int groundBeamsCells = 3; // ex: 3 means 3x3
        
        // This function is required by IDeclareReferencedPrefabs
        public void DeclareReferencedPrefabs(List<UnityGameObject> referencedPrefabs)
        {
            referencedPrefabs.Add(beamPrefab);
        }
        
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new BuildingSpawnerData
            {
                beamPrefab = conversionSystem.GetPrimaryEntity(beamPrefab),
                buildingCount = buildingCount,
                buildingAreaSize = buildingAreaSize,
                buildingMaxFloorCount = buildingMaxFloorCount,
                groundBeamsPointCount = groundBeamsPointCount,
                groundBeamsAreaSize = groundBeamsAreaSize,
                groundBeamsCells = groundBeamsCells
            });
        }
    }
}
