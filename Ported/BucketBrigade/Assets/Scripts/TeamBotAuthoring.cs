using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class TeamBotAuthoring : MonoBehaviour
{
    class Baker : Baker<TeamBotAuthoring>
    {
        public override void Bake(TeamBotAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new Teambot
            {
                State = TeamBotState.Init,
                TravelSpeed = 5f,
                DouseRadius = 7,
                MaxDouseAmount = 1.8f,
                waterFillDuration = 2,
                waterFillElapsedTime = 0,
                WaterGatherSpeed = 0.1f,
                    
            });
            
        }
    }
}

public enum TeamBotState
{
    Init,
    Idle,
    WaterHolder,
}

public enum TeamBotRole
{
    WaterGatherer,
    PassTowardsFire,
    FireDouser,
    PassTowardsWater,
}

public struct Teambot : IComponentData
{
    public TeamBotState State;
    public TeamBotRole Role;
    public Entity TeamWaterGatherer;
    public Entity TeamFireDouser;
    public Entity PassToTarget;
    public int TeamID;
    public float PositionInLine;
    
    public float3 TargetPosition;
    public Entity TargetWaterEntity;
    public Entity TargetFireEntity;

    public float TravelSpeed;

    public float DouseRadius;
    public float MaxDouseAmount;

    public float waterFillDuration;
    public float waterFillElapsedTime;
    
    public float WaterGatherSpeed;
    // public float t;
    //
    // public float3 TargetPos;
    // public Entity TargetWaterEntity;
    // public Entity TargetFireEntity;
    //
    // public OmnibotState OmnibotState;
    // public float TravelSpeed;
    //
    // public float MaxWaterCapacity;
    // public float WaterGatherSpeed;
    // public float CurrentWaterCarryingVolume;
    //
    // public float DouseRadius;
    // public float MaxDouseAmount;
}