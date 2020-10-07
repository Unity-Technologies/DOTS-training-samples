using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;

[MaterialProperty("_BaseColor", MaterialPropertyFormat.Float4)]
public struct MaterialOverride : IComponentData
{
    public float4 Value;
}

public class MaterialOverrideAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public UnityEngine.Color Color;

    public void Convert(Entity entity, EntityManager dstManager,
        GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new MaterialOverride
        {
            Value = new float4(Color.r, Color.g, Color.b, 1)
        });
    }
}