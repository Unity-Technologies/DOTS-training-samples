using Unity.Entities;

public enum BeeState
{
    Idle,
    GettingResource,
    ReturningToBase,
    ChasingEnemy,
}

public struct Bee : IComponentData
{
    public BeeState State;
    public Entity Target;
}
