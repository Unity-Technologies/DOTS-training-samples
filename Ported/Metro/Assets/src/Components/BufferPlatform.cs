using Unity.Entities;

public struct BufferPlatform : IBufferElementData
{
    public Entity Value;
    
    public static implicit operator Entity(BufferPlatform v) => v.Value;
    public static implicit operator BufferPlatform(Entity v) => new BufferPlatform { Value = v };
}
