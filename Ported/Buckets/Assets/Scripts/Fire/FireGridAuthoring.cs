using Unity.Entities;

namespace Fire
{
    [RequiresEntityConversion]
    public class FireGridAuthoring : UnityEngine.MonoBehaviour, IConvertGameObjectToEntity
    {
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            // Add buffer fire grid buffer
            // See dynamic buffer doc
            // https://docs.unity3d.com/Packages/com.unity.entities@0.0/manual/dynamic_buffers.html
            dstManager.AddBuffer<FireBufferElement>(entity);

            // Add fire buffer component data for singleton retrieval
            dstManager.AddComponentData(entity, new FireBuffer());
        }
    }

    public struct FireBuffer : IComponentData
    {
    }

    public struct FireBufferElement : IBufferElementData
    {
        public Entity FireEntity;
    }

    public struct FireBufferMetaData : IComponentData
    {
        public int CountX;
        public int CountZ;
        public int TotalSize;
    }
}