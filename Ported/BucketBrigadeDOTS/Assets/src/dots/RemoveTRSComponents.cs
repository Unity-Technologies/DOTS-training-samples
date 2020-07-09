using Unity.Entities;
using Unity.Transforms;

public class RemoveTRSComponents : UnityEngine.MonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.RemoveComponent<Translation>(entity);
        dstManager.RemoveComponent<Rotation>(entity);
        dstManager.RemoveComponent<Scale>(entity);
        dstManager.RemoveComponent<NonUniformScale>(entity);
    }
}