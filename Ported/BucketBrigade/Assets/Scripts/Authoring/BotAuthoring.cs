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
            AddComponent<BotOmni>();
            AddComponent<URPMaterialPropertyBaseColor>();
            AddComponent<BotCommand>();
            AddComponent<DecisionTimer>();
            AddComponent<TargetBucket>();
            AddComponent<TargetWater>();
            AddComponent<TargetFlame>();
            AddComponent<ArriveThreshold>();
        }
    }
}

public struct BotOmni : IComponentData { }
public struct BotScooper : IComponentData { }
public struct BotThrower : IComponentData { }
public struct BotPasser : IComponentData { }

public struct BotCommand : IComponentData
{
    public BotAction Value;
}

public struct DecisionTimer : IComponentData 
{ 
    public float value;
}

public struct CarryingBucket : IComponentData
{
    public Entity value;
}

public struct TargetBucket : IComponentData
{
    public Entity value;
}

public struct TargetWater : IComponentData
{
    public Entity value;
}

public struct TargetFlame : IComponentData
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