using Unity.Entities;
using Unity.Rendering;
using Unity.Mathematics;
using UnityEngine;

[MaterialProperty("_BaseColor", MaterialPropertyFormat.Float4)]
public struct AntColor : IComponentData
{
    public float4 Value;
}

public class AntColorAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public static readonly float4 kHungryColor = new float4(.9f, 0, .2f, 1);
    public static readonly float4 kFoodColor = new float4(0f, 0, .6f, 1);

    public UnityEngine.Color Color;

    public void Convert(Entity entity, EntityManager dstManager,
        GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new AntColor
        {
            Value = new float4(Color.r, Color.g, Color.b, 1)
        });
    }
}