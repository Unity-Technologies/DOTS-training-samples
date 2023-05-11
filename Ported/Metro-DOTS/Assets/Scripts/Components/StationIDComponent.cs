using Unity.Entities;


public struct StationIDComponent : IComponentData
{
    public int StationID;
    public int LineID;
}

public struct StationQueuesElement : IBufferElementData
{
    public Entity Queue;
}