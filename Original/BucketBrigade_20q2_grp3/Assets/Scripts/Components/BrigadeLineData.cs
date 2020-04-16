using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public struct BrigadeInitInfo : IComponentData
{
    public int WorkerCount;
}

public struct BrigadeLine : IComponentData
{
}

public struct BrigadeLineEstablished : IComponentData
{
}


public struct WorkerEntityElementData : IBufferElementData
{
    public Entity Value;
}