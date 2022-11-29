using Unity.Entities;

public enum MovementDirection
{
    North = 0,
    East = 1,
    South = 2,
    West = 3
}
public struct UnitMovementComponent : IComponentData
{
    public float speed;
    public MovementDirection direction;
}