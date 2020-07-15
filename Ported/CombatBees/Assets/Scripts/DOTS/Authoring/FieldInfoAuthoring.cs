using UnityEngine;
using Unity.Entities;
using Unity.Mathematics; 

[RequiresEntityConversion]
public class FieldInfoAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public float Height = 10;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var t = GetComponent<Transform>(); 
        var pos = t.position;
        var scale = t.lossyScale;
        var halfHeight = Height * 0.5f;

        Debug.Log($"pos{pos}, scale{scale}, halfHeight{halfHeight}"); 

        dstManager.AddComponentData(entity, new FieldInfo
        {
            Bounds = new Bounds
            {
                Center = new float3(pos.x, pos.y + halfHeight, pos.z),
                Extents = new float3(scale.x * 0.5f, halfHeight, scale.z * 0.5f)
            }
        });
    }
}

public struct FieldInfo : IComponentData
{
    public Bounds Bounds;
}

