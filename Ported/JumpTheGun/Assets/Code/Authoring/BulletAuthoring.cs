
using Unity.Entities;
using UnityMonoBehaviour = UnityEngine.MonoBehaviour;
using Unity.Mathematics;

public class BulletAuthoring : UnityMonoBehaviour
    , IConvertGameObjectToEntity
{

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new Bullet());
        dstManager.AddComponentData(entity, new Direction() { Value = new float3(0.0f, 0.0f, 0.0f) });
        dstManager.AddComponentData(entity, new WasHit() { Count = 0 });
        dstManager.AddComponentData(entity, new TargetPosition() { Value = new float3(0.0f, 0.0f, 0.0f) });
        dstManager.AddComponentData(entity, new Arc() { Value = new float3(0.0f, 0.0f, 0.0f) });
        dstManager.AddComponentData(entity, new Time() { StartTime = 0.0f, EndTime= 0.0f });
    }
}
