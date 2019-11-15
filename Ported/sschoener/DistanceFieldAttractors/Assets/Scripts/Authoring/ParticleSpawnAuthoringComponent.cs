using System;
using Unity.Entities;
using UnityEngine;

namespace Systems {
    public class ParticleSpawnAuthoringComponent : MonoBehaviour, IConvertGameObjectToEntity
    {
        public int ParticleCount = 4000;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new SpawnParticleComponent
            {
                Count = ParticleCount
            });
        }
    }
}
