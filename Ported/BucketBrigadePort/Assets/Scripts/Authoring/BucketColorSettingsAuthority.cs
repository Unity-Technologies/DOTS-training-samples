using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[GenerateAuthoringComponent]

public struct BucketColorSettings : IComponentData
{
    public float4 Empty;
    public float4 Full;
}


// TODO: This is not showing the colorpicker in the editor. To fix.
public class BucketColorSettingsAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public UnityEngine.Color BucketEmpty; // = new UnityEngine.Color(255, 105, 117, 1);
    public UnityEngine.Color BucketFull; // = new UnityEngine.Color(0, 250, 255, 1);


    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new BucketColorSettings
        {
            Empty = new float4(BucketEmpty.r, BucketEmpty.g, BucketEmpty.b, BucketEmpty.a),
            Full = new float4(BucketFull.r, BucketFull.g, BucketFull.b, BucketFull.a)
        });
    }
}