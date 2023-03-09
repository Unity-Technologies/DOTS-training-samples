using Components;
using Enums;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;
using UnityEngine.Rendering;

public class BotAuthoring : MonoBehaviour
{
    class Baker : Baker<BotAuthoring>
    {
        public override void Bake(BotAuthoring bucketAuthoring)
        {
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

public struct BucketProvider : IComponentData
{
    public Entity Value;
}

public struct DecisionTimer : IComponentData 
{ 
    public float Value;
}

public struct CarryingBucket : IComponentData
{
    public Entity Value;
}

public struct TargetBucket : IComponentData
{
    public Entity Value;
}

public struct TargetWater : IComponentData
{
    public Entity Value;
}

public struct TargetFlame : IComponentData
{
    public Entity Value;
}

public struct LocationPickup : IComponentData
{
    public float3 Value;
}

public struct LocationDropoff : IComponentData
{
    public float3 Value;
}

public struct ArriveThreshold : IComponentData
{
    public float Value;
}