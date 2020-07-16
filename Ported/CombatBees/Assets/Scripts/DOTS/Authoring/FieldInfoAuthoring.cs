using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

[RequiresEntityConversion]
public class FieldInfoAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public float Height = 10;
    public Color TeamColor;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var t = GetComponent<Transform>();
        var pos = t.position; 
        var filter = GetComponent<MeshFilter>(); 
        var mesh = filter.sharedMesh;
        var meshBounds = mesh.bounds;
        var scale = t.lossyScale;
        var halfHeight = Height * 0.5f;

        dstManager.AddComponentData(entity, new FieldInfo
        {
            Bounds = new Bounds
            {
                Center = new float3(pos.x, pos.y + halfHeight, pos.z),
                Extents = new float3(meshBounds.extents.x * scale.x, halfHeight * scale.y, meshBounds.extents.z * scale.z)
            },
            TeamColor = TeamColor
        });
    }
}

public struct FieldInfo : IComponentData
{
    public Bounds Bounds;
    public Color TeamColor;
}

