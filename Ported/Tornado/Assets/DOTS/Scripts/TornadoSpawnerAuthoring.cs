using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Dots
{
    [DisallowMultipleComponent]
    public class TornadoSpawnerAuthoring : MonoBehaviour
        , IConvertGameObjectToEntity
        , IDeclareReferencedPrefabs
    {
        public UnityEngine.GameObject particlePrefab;
        public int particleCount = 600;
        public int spinRate = 37;
        public int upwardSpeed = 6;
        public int initRange = 10;
        public float force = 0.022f;
        public float maxForceDist = 30;
        public float rotationModulation = 0;
        public int height = 50;
        public float upForce = 1.4f;
        public float inwardForce = 9f;

        // This function is required by IDeclareReferencedPrefabs
        public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
        {
            referencedPrefabs.Add(particlePrefab);
        }

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            var particle = conversionSystem.GetPrimaryEntity(particlePrefab);
            dstManager.AddComponentData(entity, new TornadoConfigData()
            {
                position = transform.position,
                particlePrefab = particle,
                particleCount = particleCount,
                spinRate = spinRate,
                upwardSpeed = upwardSpeed,
                initRange = initRange,
                force = force,
                maxForceDist = maxForceDist,
                rotationModulation = rotationModulation,
                height = height,
                upForce = upForce,
                inwardForce = inwardForce
            });
        }
    }
}
