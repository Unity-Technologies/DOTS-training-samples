using Unity.Entities;
using Unity.Mathematics;

[InternalBufferCapacity(32)]
public struct Member : IBufferElementData
{
    public static implicit operator Entity(Member e) { return e.Value; }
    public static implicit operator Member(Entity e) { return new Member { Value = e }; }

    public Entity Value;
}