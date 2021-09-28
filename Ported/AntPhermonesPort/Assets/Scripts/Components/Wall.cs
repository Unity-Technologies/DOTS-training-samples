using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public struct Wall : IComponentData
{
    public float CenterOfOpening;
    public bool DoubleOpening;
    public int WallIndex;
}