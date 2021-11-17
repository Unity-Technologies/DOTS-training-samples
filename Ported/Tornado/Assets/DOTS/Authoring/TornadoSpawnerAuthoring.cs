using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Dots
{
    public class TornadoSpawnerAuthoring : MonoBehaviour
        , IConvertGameObjectToEntity
    {
        public Mesh mesh;
        public Material material;

        public int debrisCount = 1000;
        public bool simulate = true;
        public float spinRate = 35f;
        public float upwardSpeed = 6;
        public float initRange = 10;
        [Range(0, 1)]public float force = 0.022f;
        public float maxForceDist = 30;
        public float height = 50;
        public float upForce = 1.4f;
        public float inwardForce = 9;

        // This function is required by IConvertGameObjectToEntity
        public void Convert(Entity entity, EntityManager dstManager
            , GameObjectConversionSystem conversionSystem)
        {
            // GetPrimaryEntity fetches the entity that resulted from the conversion of
            // the given GameObject, but of course this GameObject needs to be part of
            // the conversion, that's why DeclareReferencedPrefabs is important here.
            dstManager.AddComponentData(entity, new TornadoSpawner()
            {
                mesh = mesh,
                material = material,
                debrisCount = debrisCount,
                simulate = simulate,
                spinRate = spinRate,
                upwardSpeed = upwardSpeed,
                initRange = initRange,
                force = force,
                maxForceDist = maxForceDist,
                height = height,
                upForce = upForce,
                inwardForce = inwardForce
            });
        }
    }
}
