using Combatbees.Testing.Maria;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;

[DisallowMultipleComponent]
public class BeeAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public TeamName TeamName = TeamName.A;
    public float3 HomePosition = float3.zero;
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
            Value = new Random((uint) (entity.Index + 1)) // +1 because seed can't be 0
        });

        dstManager.AddComponentData(entity, new Agression
        {
            Value = 1f
        });

        dstManager.AddComponentData(entity, new Targeted
        {
            Value = false
        });
        dstManager.AddComponentData(entity, new Falling()
        {
            shouldFall = false,
            timeToLive = 5f
        });
    }
}
