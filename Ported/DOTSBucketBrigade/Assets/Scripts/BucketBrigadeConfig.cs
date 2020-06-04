using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[GenerateAuthoringComponent]
public struct BucketBrigadeConfig : IComponentData
{
    public float TemperatureIncreaseRate;
    public float Flashpoint;
    public int2 GridDimensions;
    public float CellSize;

    public float WaterSourceRefillRate;
    public float BucketCapacity;
    public float BucketRadius;
    public float AgentRadius;
    public float AgentSpeed;
    public int NumberOfBuckets;

    public int NumberOfPassersInOneDirectionPerChain;
    public int NumberOfChains;

    public int StartingFireCount;
    public float MaxFlameHeight;
    public int HeatRadius;
    public float FlickerRate;
    public float FlickerRange;

    public float MovementTargetReachedThreshold;
    public float CarriedBucketHeightOffset;

    public int SplashRadius;
    public float CoolingStrength;
    public float CoolingFallOff;

    public Color ColorNeutral;
    public Color ColorCool;
    public Color ColorHot;

}
