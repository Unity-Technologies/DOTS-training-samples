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

    //public float Angle;
    //public float3 Forward;
    //public float3 DestinationDirection;
}

public struct TrainSpawn : IComponentData
{
    public Entity CarriageSpawn;
    public int CarriageCount;
}