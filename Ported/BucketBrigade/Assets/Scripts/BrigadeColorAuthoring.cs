using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;

public struct BrigadeColor : IComponentData
{
    public float4 tossColor;
    public float4 scoopColor;
    public float4 emptyColor;
    public float4 fullColor;
}

[MaterialProperty("_BaseColor", MaterialPropertyFormat.Float4)]
public struct BotColor : IComponentData
{
    public float4 Value;
}

public class BrigadeColorAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public UnityEngine.Color tossColor;
    public UnityEngine.Color scoopColor;
    public UnityEngine.Color emptyColor;
    public UnityEngine.Color fullColor;
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new BrigadeColor()
        {
            tossColor = ColorToFloat4(tossColor),
            scoopColor = ColorToFloat4(scoopColor),
            emptyColor = ColorToFloat4(emptyColor),
            fullColor = ColorToFloat4(fullColor)
        });
    }
    public float4 ColorToFloat4(UnityEngine.Color c)
    {
        return new float4(c.r, c.g, c.b, c.a);
    }
}