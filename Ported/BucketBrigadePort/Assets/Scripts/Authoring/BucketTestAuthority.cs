using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct BucketTestSettings : IComponentData
{
    public float4 ColorFireNeutral;
    public float4 ColorFireCool;
    public float4 ColorFireHot;
    public float FlameHeight;
    public float FlickerRate;
    public float FlickerRange;

}

public class BucketTestSettingsAuthoring : UnityEngine.MonoBehaviour, IConvertGameObjectToEntity
{
    public UnityEngine.Color ColorFireNeutral;
    public UnityEngine.Color ColorFireCool;
    public UnityEngine.Color ColorFireHot;
    public float FlameHeight = 10f;
    public float FlickerRate = 0.1f;
    public float FlickerRange = 0.1f;
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new BucketTestSettings
        {
            ColorFireNeutral = new float4(ColorFireNeutral.r, ColorFireNeutral.g, ColorFireNeutral.b, ColorFireNeutral.a),
            ColorFireCool = new float4(ColorFireCool.r, ColorFireCool.g, ColorFireCool.b, ColorFireCool.a),
            ColorFireHot = new float4(ColorFireHot.r, ColorFireHot.g, ColorFireHot.b, ColorFireHot.a),
            FlameHeight = FlameHeight,
            FlickerRate = FlickerRate,
            FlickerRange = FlickerRange,
        });
    }
}