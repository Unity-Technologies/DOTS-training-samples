using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityMonoBehaviour = UnityEngine.MonoBehaviour;
using UnityGameObject = UnityEngine.GameObject;
using Unity.Mathematics;

public class BulletAuthoring : UnityMonoBehaviour
    , IConvertGameObjectToEntity
    , IDeclareReferencedPrefabs
{
    public UnityGameObject BulletPrefab;

    public void DeclareReferencedPrefabs(List<UnityEngine.GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(BulletPrefab);
    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new Bullet()
        {
            BulletPrefab = conversionSystem.GetPrimaryEntity(BulletPrefab)
        });
        dstManager.AddComponentData(entity, new Translation() { Value = new float3(0.0f, 0.0f, 0.0f) });
        dstManager.AddComponentData(entity, new Direction() { Value = new float3(0.0f, 0.0f, 0.0f) });
        dstManager.AddComponentData(entity, new WasHit() { Count = 0 });
        dstManager.AddComponentData(entity, new TargetPosition() { Value = new float3(0.0f, 0.0f, 0.0f) });
        dstManager.AddComponentData(entity, new Arc() { Value = new float3(0.0f, 0.0f, 0.0f) });
        dstManager.AddComponentData(entity, new Time() { Value = 0.0f });
    }
}
