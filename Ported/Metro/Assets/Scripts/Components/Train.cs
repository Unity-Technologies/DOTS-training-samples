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
    public TrainState State;

    public NativeArray<Entity> Carriage;
}