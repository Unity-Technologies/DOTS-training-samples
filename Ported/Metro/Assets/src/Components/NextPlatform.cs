using Unity.Entities;

public struct NextPlatform : IComponentData
{
    public Entity Value;
            
    public static implicit operator Entity(NextPlatform v) => v.Value;
    public static implicit operator NextPlatform(Entity v) => new NextPlatform { Value = v };
}
