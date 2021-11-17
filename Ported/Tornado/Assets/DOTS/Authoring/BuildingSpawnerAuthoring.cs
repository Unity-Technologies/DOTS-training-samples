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
        [UnityRange(1, 500)]
        public int buildingCount = 35;

        public bool useBeamGroups = true;
        
        // This function is required by IDeclareReferencedPrefabs
        public void DeclareReferencedPrefabs(List<UnityGameObject> referencedPrefabs)
        {
            referencedPrefabs.Add(beamPrefab);
        }
        
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new BuildingSpawnerData
            {
                BeamPrefab = conversionSystem.GetPrimaryEntity(beamPrefab),
                BuildingCount = buildingCount,
                UseBeamGroups = useBeamGroups
            });
        }
    }
}
