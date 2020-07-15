using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Fire
{
    public class FireAuthoring : UnityEngine.MonoBehaviour, IConvertGameObjectToEntity
    {
        public float SizeXZ = 1;
        public float SizeY = 5;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new Translation { Value = new float3() });
            dstManager.AddComponentData(entity, new TemperatureComponent { Value = 0f });
            dstManager.AddComponentData(entity, new BoundsComponent { Size = SizeXZ, Height = SizeY});
        }
    }

    public struct TemperatureComponent : IComponentData
    {
        public float Value;
    }

    // TODO make bounds readonly once initialized
    public struct BoundsComponent : IComponentData
    {
        public float Size;
        public float Height;
    }

    // Note: added at runtime - used to signify "On Fire" state
    public struct TemperatureVelocity : IComponentData
    {
        public float Value;
    }
}
