
using Unity.Entities;
using UnityMonoBehaviour = UnityEngine.MonoBehaviour;
using Unity.Mathematics;

public class BulletAuthoring : UnityMonoBehaviour
    , IConvertGameObjectToEntity
{

    public static void CreateBulletArchetype(Entity entity, EntityManager entityManager)
    {
        entityManager.AddComponentData(entity, new Bullet());
        entityManager.AddComponentData(entity, new Direction() { Value = new float3(0.0f, 0.0f, 0.0f) });
        entityManager.AddComponentData(entity, new WasHit() { Count = 0 });
        entityManager.AddComponentData(entity, new TargetPosition() { Value = new float3(0.0f, 0.0f, 0.0f) });
        entityManager.AddComponentData(entity, new Arc() { Value = new float3(0.0f, 0.0f, 0.0f) });
        entityManager.AddComponentData(entity, new Time() { StartTime = 0.0f, EndTime= 0.0f });
    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        CreateBulletArchetype(entity, dstManager);
    }
}
