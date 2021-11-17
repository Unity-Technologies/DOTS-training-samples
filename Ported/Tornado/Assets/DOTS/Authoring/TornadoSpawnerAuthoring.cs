using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityGameObject = UnityEngine.GameObject;
using UnityRangeAttribute = UnityEngine.RangeAttribute;
using UnityMonoBehaviour = UnityEngine.MonoBehaviour;

namespace Dots
{
    [DisallowMultipleComponent]
    public class TornadoSpawnerAuthoring : UnityMonoBehaviour
        , IConvertGameObjectToEntity
        , IDeclareReferencedPrefabs
    {
        public bool simulate = true;
        
        public float spinRate = 37;
        public float upwardSpeed = 6;
        public float initRange = 10f;

        [Range(0f, 1f)] public float force = 0.022f;
        public float maxForceDist = 30f;
        public float height = 50f;
        public float upForce = 1.4f;
        public float inwardForce = 9;
        public float rotationModulation = 0;
        
        public UnityGameObject debrisPrefab;
        [Range(1f, 10000f)] public float debrisCount = 1000;
        public float debrisInitRange = 10f;
        
        // This function is required by IDeclareReferencedPrefabs
        public void DeclareReferencedPrefabs(List<UnityGameObject> referencedPrefabs)
        {
            if (debrisPrefab != null)
                referencedPrefabs.Add(debrisPrefab);
        }
        
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new TornadoConfig
            {
                simulate = simulate,
                spinRate = spinRate,
                upwardSpeed = upwardSpeed,
                initRange = initRange,
                force = force,
                maxForceDist = maxForceDist,
                height = height,
                upForce = upForce,
                inwardForce = inwardForce,
                rotationModulation = rotationModulation,
                initialPosition = transform.position
            });
            dstManager.AddComponentData(entity, new TornadoFader { value = 0f });
            
            dstManager.AddComponentData(entity, new DebrisSpawnerData
            {
                debrisPrefab = conversionSystem.GetPrimaryEntity(debrisPrefab),
                debrisCount = debrisCount,
                initRange = debrisInitRange,
                height = height
            });
        }
    }
}
