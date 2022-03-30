using Unity.Entities;

public struct TargetType : IComponentData
{
    public enum Type : uint
    {
        None,
        Enemy,
        Resource,
        Goal
    };

    public Type Value;
}
