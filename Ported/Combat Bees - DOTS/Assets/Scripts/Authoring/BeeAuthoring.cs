using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;


[DisallowMultipleComponent]
public class BeeAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public TeamName TeamName = TeamName.A;
    public float3 HomePosition = float3.zero;
    public float ColliderRadius = 0.5f;
    
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponent<BeeTag>(entity);
        
        dstManager.AddComponentData(entity, new BeeTargets
        {
            ResourceTarget = Entity.Null,
            EnemyTarget = Entity.Null,
            HomePosition = HomePosition,
            CurrentTargetPosition = float3.zero
        });
        
        dstManager.AddComponentData(entity, new BeeDead()
        {
            Value = false,
            AnimationStarted = false,
        });

        dstManager.AddComponentData(entity, new Velocity
        {
            Value = float3.zero
        });

        dstManager.AddComponentData(entity, new Team
        {
            Value = TeamName
        });

        dstManager.AddComponentData(entity, new SmoothPosition
        {
            Value = float3.zero
        });

        dstManager.AddComponentData(entity, new HeldItem
        {
            Value = Entity.Null
        });

        dstManager.AddComponentData(entity, new BeeStatus
        {
            Value =  Status.Idle
        });
        
        dstManager.AddComponentData(entity, new RandomState
        {
            // Value = new Random((uint) (entity.Index + 1)) // +1 because seed can't be 0 --> Only 2 entities in the game (so this won't work)
            Value = new Unity.Mathematics.Random((uint)UnityEngine.Random.Range(1, 100000))
        });
        
        dstManager.AddComponentData(entity, new Agression
        {
            Value = -1 // this value will be inited somewhere else: in BeeInitSystem
        });
        
        dstManager.AddComponentData(entity, new Falling()
        {
            shouldFall = false,
            timeToLive = 5f
        });

        dstManager.AddComponentData(entity, new ColliderRadius
        {
            Value = ColliderRadius
        });

        dstManager.AddComponentData(entity, new NonUniformScale
        {
            Value = new float3(1f,1f,1f)
        });
    }
}
