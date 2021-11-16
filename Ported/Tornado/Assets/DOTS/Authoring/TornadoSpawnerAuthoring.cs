using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityGameObject = UnityEngine.GameObject;
using UnityRangeAttribute = UnityEngine.RangeAttribute;
using UnityMonoBehaviour = UnityEngine.MonoBehaviour;

namespace Dots
{
    [DisallowMultipleComponent]
    public class TornadoSpawnerAuthoring : UnityMonoBehaviour
        , IConvertGameObjectToEntity
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
        
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new Dots.TornadoConfig
            {
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
            dstManager.AddComponentData(entity, new TornadoFader { value = 0f });
        }
    }
}
