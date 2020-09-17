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

    public static readonly float4 kHungryColor = new float4(0.2677333f, 0.3012f, 0.502f, 1);
    public static readonly float4 kFoodColor = new float4(0.78f, 0.7672826f, 0.4281521f, 1) * 2;
    public static readonly float4 kSeeFoodColor = new float4(0.0f, 1.0f, 0.0f, 1);
    public static readonly float4 kSeeBaseColor = new float4(1.0f, 0.0f, 0.0f, 1);

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