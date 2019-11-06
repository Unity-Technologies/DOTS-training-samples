using ECSExamples;
using Unity.Entities;
using Unity.Mathematics;

public struct AiPlayerComponent : IComponentData
{
    public float3 TargetPosition;
    public float3 CurrentPosition;
    public Direction Direction;
    public float2 CellCoordinate;
    public float StartTime;
}