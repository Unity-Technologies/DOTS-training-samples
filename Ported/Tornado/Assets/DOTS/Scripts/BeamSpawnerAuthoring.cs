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
        public float friction = 0.4f;
        public float damping = 0.012f;
        public float expForce = 0.4f;
        public float breakResistance = 0.55f;

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
                debrisCount = debrisCount,
                friction = friction,
                damping = damping,
                expForce = expForce,
                breakResistance = breakResistance
            });
        }
    }
}
