using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct Position : IComponentData
{
    public float2 Value;
}

public class PositionAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new Position
        {
            Value = new float2(transform.position.x, transform.position.z)
        });
    }
}