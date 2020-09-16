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