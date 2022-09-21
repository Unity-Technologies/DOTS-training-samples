using Unity.Entities;
using Unity.Mathematics;

public struct RandomMovement : IComponentData
{
    public int2 TargetPosition;
}
