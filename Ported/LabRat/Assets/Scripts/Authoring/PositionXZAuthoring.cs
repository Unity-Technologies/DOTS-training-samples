using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

[WriteGroup(typeof(LocalToWorld))]
public struct PositionXZ : IComponentData
{
    public float2 Value;
}

public class PositionXZAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager dstManager,
        GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new PositionXZ()
        {
            Value = new float2(transform.position.x, transform.position.z)
        });
        dstManager.RemoveComponent<Translation>(entity);

    }
}