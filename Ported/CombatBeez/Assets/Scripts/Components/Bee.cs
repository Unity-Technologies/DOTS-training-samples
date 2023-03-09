using Unity.Entities;
using Unity.Mathematics;

public struct Bee : IComponentData
{
    public float3 OcillateOffset;
    public float3 Target;
    public Entity TargetResource;
    public Entity TargetBee;
    public BEESTATE beeState;
    public Team beeTeam;
    public float3 SpawnPoint;
    public bool AtTarget;

    public enum BEESTATE
    {
        IDLE,
        FORAGE,
        CARRY,
        ATTACK
    };
}
