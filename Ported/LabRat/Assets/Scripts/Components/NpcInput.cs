using Unity.Entities;
using Unity.Mathematics;

public struct NpcInput : IComponentData
{
    public Random random;
    public float nextMoveDelay;
    public float timeSinceLastMove;
}
