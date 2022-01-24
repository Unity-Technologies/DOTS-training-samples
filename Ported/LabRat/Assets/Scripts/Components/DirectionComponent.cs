using Unity.Entities;


public struct DirectionComponent : IComponentData
{
    public enum DirectionEnum
    {
        FORWARD,
        BACKWARD,
        LEFT,
        RIGHT,
    }

    public DirectionEnum dir;

    public DirectionComponent(DirectionEnum direction)
    {
        dir = direction;
    }
}
