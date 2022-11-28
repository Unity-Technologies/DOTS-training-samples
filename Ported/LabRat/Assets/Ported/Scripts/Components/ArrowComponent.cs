using Unity.Entities;
using Unity.Mathematics;

public struct ArrowComponent : IComponentData
{
    public float2 position;
    public int direction;
    public int team;
}
