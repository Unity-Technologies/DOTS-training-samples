using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class TransformAuthoring2D : UnityEngine.MonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new Translation2D {Value = ((float3) transform.position).xy});
        // dstManager.AddComponentData(entity, new Scale2D {Value = ((float3) transform.localScale).xy});

        dstManager.RemoveComponent<Translation>(entity);
        dstManager.RemoveComponent<Rotation>(entity);
        dstManager.RemoveComponent<Scale>(entity);
        dstManager.RemoveComponent<NonUniformScale>(entity);
    }
}

public struct Velocity2D : IComponentData
{
    public float2 Value;
}

[WriteGroup(typeof(LocalToWorld))]
public struct Translation2D : IComponentData
{
    public float2 Value;
}

[WriteGroup(typeof(LocalToWorld))]
public struct Scale2D : IComponentData
{
    public float2 Value;
}
