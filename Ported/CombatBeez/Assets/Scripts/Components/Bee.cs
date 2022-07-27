using Unity.Entities;
using Unity.Mathematics;

public struct Bee : IComponentData
{
    public float3 OcillateOffset;
    public float3 Target;
    public Entity TargetResource;
    public BEESTATE beeState;

    public float3 SpawnPoint;

    public enum BEESTATE
    {
        IDLE,
        FORAGE,
        CARRY,
        ATTACK
    };
}
