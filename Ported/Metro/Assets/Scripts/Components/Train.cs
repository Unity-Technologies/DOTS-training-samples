using Unity.Collections;
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
    public float2 Destination;
    public float DistanceToNextTrain;
    public float2 Direction;
    public TrainState State;
}

public struct TrainSpawn : IComponentData
{
    public Entity CarriageSpawn;
    public int CarriageCount;
}