using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Dots
{
    [DisallowMultipleComponent]
    public class WorldConfigAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        public float breakResistance = 0.55f;
        [Range(0f, 1f)] public float damping = 0.012f;
        [Range(0f, 1f)] public float friction = 0.4f;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new WorldConfig
            {
                breakResistance = breakResistance,
                damping = damping,
                friction = friction
            });
        }
    }
}
