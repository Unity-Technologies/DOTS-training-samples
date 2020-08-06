using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class Transform2DAuthoring : UnityEngine.MonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new Position { value = ((float3)transform.position).xy });
        dstManager.AddComponentData(entity, new Scale { value = ((float3)transform.localScale).xy });

        dstManager.RemoveComponent<Translation>(entity);
        dstManager.RemoveComponent<Rotation>(entity);
        dstManager.RemoveComponent<Scale>(entity);
        dstManager.RemoveComponent<NonUniformScale>(entity);
    }
}

[WriteGroup(typeof(LocalToWorld))]
public struct Scale : IComponentData
{
    public float2 value;
}
