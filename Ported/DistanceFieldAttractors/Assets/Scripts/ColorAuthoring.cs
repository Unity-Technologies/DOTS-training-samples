using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;

[MaterialProperty("_BaseColor", MaterialPropertyFormat.Float4)]
public struct Color : IComponentData
{
    public float4 Value;
}

public class ColorAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public UnityEngine.Color color = UnityEngine.Color.black;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new Color
        {
            Value = new float4(color.r, color.g, color.b, color.a)
        });
    }
}
