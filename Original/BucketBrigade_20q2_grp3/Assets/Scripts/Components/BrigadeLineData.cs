using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public struct BrigadeInitInfo : IComponentData
{
    public int WorkerCount;
}

public struct BrigadeLine : IComponentData
{
    public float2 Center;
}

public struct BrigadeLineEstablished : IComponentData
{
}


public struct WorkerEntityElementData : IBufferElementData
{
    public Entity Value;
}