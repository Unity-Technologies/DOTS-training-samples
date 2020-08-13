using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct BucketColorSettings : IComponentData
{
    public float4 Empty;
    public float4 Full;
}


// public class TileDisplaySettingsAuthoring : UnityEngine.MonoBehaviour, IConvertGameObjectToEntity
public class BucketColorSettingsAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public UnityEngine.Color Empty; // = new UnityEngine.Color(255, 105, 117, 1);
    public UnityEngine.Color Full; // = new UnityEngine.Color(0, 250, 255, 1);


    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new BucketColorSettings
        {
            Empty = new float4(Empty.r, Empty.g, Empty.b, Empty.a),
            Full = new float4(Full.r, Full.g, Full.b, Full.a)
        });
    }
}