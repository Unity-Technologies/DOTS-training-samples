using Unity.Entities;

namespace Fire
{
    public class FireGridAuthoring : UnityEngine.MonoBehaviour, IConvertGameObjectToEntity
    {
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            // Add buffer fire grid buffer
            dstManager.AddBuffer<FireBufferElement>(entity);
        }
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