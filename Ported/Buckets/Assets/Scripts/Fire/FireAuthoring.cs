using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Fire
{
    [RequiresEntityConversion]
    public class FireAuthoring : UnityEngine.MonoBehaviour, IConvertGameObjectToEntity
    {
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new TemperatureComponent { Value = 0f });

            dstManager.AddComponentData(entity, new StartHeight { Value = 0f });
            dstManager.AddComponentData(entity, new BoundsComponent
            {
                SizeXZ = transform.localScale.x,
                SizeY = transform.localScale.y
            });
        }
    }

    public struct StartHeight : IComponentData
    {
        public float Value;
    }

    public struct TemperatureComponent : IComponentData
    {
        public float Value;
        public float Velocity;
    }

    // TODO make bounds readonly once initialized
    public struct BoundsComponent : IComponentData
    {
        public float SizeXZ;
        public float SizeY;
    }
}
