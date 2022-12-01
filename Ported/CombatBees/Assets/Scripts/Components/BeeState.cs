using Unity.Entities;
using Unity.Mathematics;

public struct BeeState : IComponentData
{
    public BeeStateEnumerator beeState;
    public float3 velocity;
    public float deathTimer;
}

public enum BeeStateEnumerator
{
    Idle,
    Gathering,
    CarryBack,
    Attacking,
    Dying,
}