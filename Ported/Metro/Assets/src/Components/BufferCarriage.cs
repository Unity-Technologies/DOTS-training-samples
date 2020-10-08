using Unity.Entities;

public struct BufferCarriage : IBufferElementData
{
    public Entity Value;
    
    public static implicit operator Entity(BufferCarriage v) => v.Value;
    public static implicit operator BufferCarriage(Entity v) => new BufferCarriage { Value = v };
}
