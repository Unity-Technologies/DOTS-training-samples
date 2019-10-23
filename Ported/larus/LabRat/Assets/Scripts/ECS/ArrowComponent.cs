using ECSExamples;
using Unity.Entities;
using Unity.Mathematics;

public struct ArrowComponent : IComponentData
{
    public Direction Direction;
    public float2 Coordinates;
    public int PlayerId;
    public int PlacementTick;
}
