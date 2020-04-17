using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[RequiresEntityConversion]
public class SegmentInfoAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public int Order;
    public SegmentShape SegmentShape;
    
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new SegmentInfo
        {
            StartXZ = new float2(transform.localPosition.x, transform.localPosition.z),
            StartRotation = math.radians(transform.localEulerAngles.y),
            Order = Order,
            SegmentShape = SegmentShape
        });
    }
}
