using Unity.Entities;

// TODO : Think more about it
[InternalBufferCapacity(0)]
public struct EntityElement : IBufferElementData
{
    public Entity Value;

    public static implicit operator Entity(EntityElement e)
    {
        return e.Value;
    }

    public static implicit operator EntityElement(Entity entity)
    {
        return new EntityElement { Value = entity };
    }
}
