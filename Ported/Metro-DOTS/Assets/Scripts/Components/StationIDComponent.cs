using Unity.Entities;


public struct StationIDComponent : IComponentData
{
    public int StationID;
}

public struct StationQueuesElement : IBufferElementData
{
    public Entity Queue;
}