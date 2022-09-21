using Unity.Entities;
using Unity.Mathematics;

public struct Target : IComponentData
{
    public int2 TargetPosition;
    public DynamicBuffer<int2> TargetPath;
    public int PathIndex;
}