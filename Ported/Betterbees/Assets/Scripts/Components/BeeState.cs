using Unity.Entities;

public struct BeeState : IComponentData
{
    public enum State
    {
        IDLE,
        GATHERING,
        RETURNING,
        ATTACKING
    }

    public State state;
}
