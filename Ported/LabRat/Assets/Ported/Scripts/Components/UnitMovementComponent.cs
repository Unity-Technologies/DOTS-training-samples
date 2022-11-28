using Unity.Entities;

public struct UnitMovementComponent : IComponentData
{
    public float speed;
    public int direction;
}