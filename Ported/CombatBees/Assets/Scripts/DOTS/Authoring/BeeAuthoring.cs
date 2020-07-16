using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[RequiresEntityConversion]
public class BeeAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public int Team;
    public float3 InitialVelocity;
    public Color Color;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new Velocity() { Value = InitialVelocity });
        dstManager.AddComponentData(entity, new Size() { Value = 1.0f });
        dstManager.AddComponentData(entity, new BeeColor() { Value = new float4(Color.r, Color.g, Color.b, Color.a) });
        dstManager.AddComponentData(entity, new NonUniformScale() { Value = new float3(1f) });
        dstManager.AddComponentData(entity, new Smoothing() { SmoothPosition = float3.zero, SmoothDirection = float3.zero });
    }
}
