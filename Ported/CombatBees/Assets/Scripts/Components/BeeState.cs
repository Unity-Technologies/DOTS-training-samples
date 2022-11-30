using Unity.Entities;
using Unity.Mathematics;

struct BeeState : IComponentData
{
    public BeeStateEnumerator beeState;
    public float3 velocity;
}

public enum BeeStateEnumerator
{
    Idle,
    Gathering,
    CarryBack,
    Attacking,
    Dying,
}