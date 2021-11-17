using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Dots
{
    [DisallowMultipleComponent]
    public class BeamSpawnerAuthoring : MonoBehaviour
        , IConvertGameObjectToEntity
        , IDeclareReferencedPrefabs
    {
        public int buildingCount = 1;
        public UnityEngine.GameObject beamPrefab;
        public int debrisCount = 600;

        // This function is required by IDeclareReferencedPrefabs
        public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
        {
            referencedPrefabs.Add(beamPrefab);
        }

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            var beamEntity = conversionSystem.GetPrimaryEntity(beamPrefab);
            dstManager.AddComponentData(entity, new BeamSpawnerData
            {                
                buildingCount = buildingCount,
                beamPrefab = beamEntity,
                debrisCount = debrisCount
            });
        }
    }
}
