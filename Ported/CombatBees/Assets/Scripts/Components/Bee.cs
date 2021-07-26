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
    public Team Team;
    public BeeState State;
    public Entity Target;
}
