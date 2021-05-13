
using System.Collections.Generic;

using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using UnityGameObject = UnityEngine.GameObject;
using Unity.Mathematics;

public class PlayerAuthoring : MonoBehaviour
    , IConvertGameObjectToEntity
{
    public float BounceTime = 0.8f;
    public float CooldownTime = 0.2f;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponent<Arc>(entity);
        dstManager.AddComponent<Direction>(entity);
        //dstManager.AddComponent<BallTrajectory>(entity);
        dstManager.AddComponent<PlayerSpawnerTag>(entity);
        dstManager.AddComponent<WasHit>(entity);
        dstManager.AddComponentData(entity, new Player(){ BounceTime = BounceTime, CooldownTime = CooldownTime });

        dstManager.AddComponentData(entity, new BallTrajectory()
        {
            Source = float3.zero,
            Destination = float3.zero
        }
        );

        dstManager.AddComponentData(entity, new BoardPosition());
        dstManager.AddComponentData(entity, new BoardTarget());
        dstManager.AddComponentData(entity, new Arc() { Value = new float3(0.0f, 0.0f, 0.0f) });
        dstManager.AddComponentData(entity, new Time() { StartTime = 0.0f, EndTime = 0.0f });
        dstManager.AddComponentData(entity, new TimeOffset() { Value = 0.0f });
    }
}
