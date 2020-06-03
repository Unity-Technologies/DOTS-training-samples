using Unity.Entities;
using Unity.Mathematics;

public struct BucketBrigadeConfig : IComponentData
{
    public float TemperatureIncreaseRate;
    public float FlashpointMax;
    public float FlashpointMin;
    public int2 GridDimensions;
    public float CellSize;

    public float WaterSourceRefillRate;
    public float BucketCapacity;
    public float BucketRadius;
    public float AgentRadius;
    public float AgentSpeed;
    public int NumberOfBuckets;
}
