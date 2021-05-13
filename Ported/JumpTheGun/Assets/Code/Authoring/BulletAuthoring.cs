
using Unity.Entities;
using UnityMonoBehaviour = UnityEngine.MonoBehaviour;
using Unity.Mathematics;
using Unity.Transforms;

public class BulletAuthoring : UnityMonoBehaviour
    , IConvertGameObjectToEntity
{

    public static void CreateBulletArchetype(Entity entity, EntityManager entityManager)
    {
        entityManager.AddComponentData(entity, new Bullet());
        entityManager.AddComponentData(entity, new CurrentLevel());
        entityManager.AddComponentData(entity, new Direction() { Value = new float3(0.0f, 0.0f, 0.0f) });
        entityManager.AddComponentData(entity, new WasHit() { Count = 0 });
        entityManager.AddComponent<NonUniformScale>(entity);
        entityManager.AddComponentData(entity, new BallTrajectory()
            {
                Source = float3.zero,
                Destination = float3.zero
            }
        );
        entityManager.AddComponentData(entity, new BoardTarget());
        entityManager.AddComponentData(entity, new Arc() { Value = new float3(0.0f, 0.0f, 0.0f) });
        entityManager.AddComponentData(entity, new Time() { StartTime = 0.0f, EndTime= 0.0f });
        entityManager.AddComponentData(entity, new TimeOffset() { Value = 0.0f });
    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        CreateBulletArchetype(entity, dstManager);
    }
}
