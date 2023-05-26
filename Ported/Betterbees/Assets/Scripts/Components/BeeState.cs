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
    public HiveTag hiveTag;
    public float aggresion;
    public float aggressionModifier;
}
