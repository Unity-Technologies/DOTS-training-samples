using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Dots
{
    public class SimulationManagerAuthoring : MonoBehaviour
        , IConvertGameObjectToEntity
    {
        public float expForce = 0.4f;
        public float breakResistance = 0.55f;
        [Range(0f, 1f)] public float damping = 0.012f;
        [Range(0f, 1f)] public float friction = 0.4f;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new SimulationManager()
            {
                expForce = expForce,
                breakResistance = breakResistance,
                damping = damping,
                friction = friction
            });
        }
    }
}

