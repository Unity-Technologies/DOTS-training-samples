using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;

[MaterialProperty("_BaseColor", MaterialPropertyFormat.Float4)]
public struct BaseColor: IComponentData
{
    public float4 Value;
}

public class BaseColorAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public Color color;

    public void Convert(Entity entity, EntityManager dstManager,
        GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new BaseColor
        {
            Value = new float4(color.r, color.g, color.b, color.a)
        });
    }
}
