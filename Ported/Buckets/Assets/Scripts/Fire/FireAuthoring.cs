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
            //dstManager.AddComponentData(entity, new Translation { Value = new float3() });
            dstManager.AddComponentData(entity, new TemperatureComponent { Value = 0f });
            dstManager.AddComponentData(entity, new BoundsComponent
            {
                SizeXZ = transform.localScale.x,
                SizeY = transform.localScale.y
            });
        }
    }

    public struct TemperatureComponent : IComponentData
    {
        public float Value;
    }

    // TODO make bounds readonly once initialized
    public struct BoundsComponent : IComponentData
    {
        public float SizeXZ;
        public float SizeY;
    }

    // Note: added at runtime - used to signify "On Fire" state
    public struct TemperatureVelocity : IComponentData
    {
        public float Value;
    }
}
