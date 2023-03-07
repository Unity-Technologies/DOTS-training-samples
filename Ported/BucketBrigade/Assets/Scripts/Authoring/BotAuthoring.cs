using Components;
using Enums;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;

public class BotAuthoring : MonoBehaviour
{
    class Baker : Baker<BotAuthoring>
    {
        public override void Bake(BotAuthoring bucketAuthoring)
        {
            AddComponent<Position>();
            AddComponent<BotRole>();
            AddComponent<Components.Color>();
            AddComponent<URPMaterialPropertyBaseColor>();
            AddComponent<BotCommand>();
            AddComponent<CommandCompleted>();
            SetComponentEnabled<CommandCompleted>(false);
            AddComponent<DecisionTimer>();
            AddComponent<TargetBucket>();
            SetComponentEnabled<TargetBucket>(false);
            AddComponent<TargetWater>();
            SetComponentEnabled<TargetWater>(false);
            AddComponent<TargetFlame>();
            SetComponentEnabled<TargetFlame>(false);
            AddComponent<ArriveThreshold>();
        }
    }
}

public struct BotRole : IComponentData
{
    public BotType value;
}

public struct BotCommand : IComponentData
{
    public BotAction value;
}

public struct CommandCompleted : IComponentData, IEnableableComponent { }

public struct DecisionTimer : IComponentData 
{ 
    public float value;
}

public struct CarryingBucket : IComponentData, IEnableableComponent
{
    public Entity value;
}

public struct TargetBucket : IComponentData, IEnableableComponent
{
    public Entity value;
}

public struct TargetWater : IComponentData, IEnableableComponent
{
    public Entity value;
}

public struct TargetFlame : IComponentData, IEnableableComponent
{
    public Entity value;
}

public struct LocationPickup : IComponentData
{
    public float3 value;
}

public struct LocationDropoff : IComponentData
{
    public float3 value;
}

public struct ArriveThreshold : IComponentData
{
    public float value;
}