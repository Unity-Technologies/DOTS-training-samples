using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

public struct QueueComponent : IComponentData
{
    public int LineID;
    public bool OnPlatformA;
    public int StartIndex;
    public int QueueLength;
    public Entity Station;
}

[InternalBufferCapacity(16)]
public struct QueuePassengers : IBufferElementData
{
    /// <summary>
    /// DO NOT USE Add ONLY USE ElementAt
    /// </summary>
    public Entity Passenger;
}