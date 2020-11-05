using Unity.Entities;
using Unity.Mathematics;

public struct BotChain : IComponentData
{
    public float3 StartChain;
    public float3 EndChain;
}

[InternalBufferCapacity(16)] // TODO: profile
public struct PasserFullBufferElement : IBufferElementData
{
    public static implicit operator Entity(PasserFullBufferElement e) { return e.bot; }
    public static implicit operator PasserFullBufferElement(Entity b) { return new PasserFullBufferElement { bot = b }; }

    public Entity bot;
}

[InternalBufferCapacity(16)] // TODO: profile
public struct PasserEmptyBufferElement : IBufferElementData
{
    public static implicit operator Entity(PasserEmptyBufferElement e) { return e.bot; }
    public static implicit operator PasserEmptyBufferElement(Entity b) { return new PasserEmptyBufferElement { bot = b }; }

    public Entity bot;
}