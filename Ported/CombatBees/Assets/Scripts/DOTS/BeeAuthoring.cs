using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[RequiresEntityConversion]
public class BeeAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new Velocity() { Value = new float3(0f) });
        dstManager.AddComponentData(entity, new Team() { Value = 0 });
        dstManager.AddComponentData(entity, new Size() { Value = 1.0f });
        dstManager.AddComponentData(entity, new BeeColor() { Value = new float4(0f) });
        dstManager.AddComponentData(entity, new NonUniformScale() { Value = new float3(1f) });
    }
}
