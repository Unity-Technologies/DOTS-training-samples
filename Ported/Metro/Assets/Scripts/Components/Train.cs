using Unity.Entities;
using Unity.Mathematics;

public enum TrainState
{
    EnRoute,
    Arriving,
    Unloading,
    Loading,
    Departing
}

public struct Train : IComponentData
{
    public int Index;

    public float3 Destination;
    public float DistanceToNextTrain;
    public TrainState State;
}

public struct TrainSpawn : IComponentData
{
    public Entity CarriageSpawn;
    public int CarriageCount;
}