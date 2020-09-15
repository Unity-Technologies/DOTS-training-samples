using Unity.Entities;

public enum DirectionEnum
{
    North,
    East,
    South,
    West
}

[GenerateAuthoringComponent]
public struct Direction : IComponentData
{
    public DirectionEnum Value;
}
