using Unity.Entities;

struct BeeState : IComponentData
{
    public BeeStateEnumerator beeState;
}

public enum BeeStateEnumerator
{
    Gathering,
    CarryBack,
    Attacking,
    Dying
}