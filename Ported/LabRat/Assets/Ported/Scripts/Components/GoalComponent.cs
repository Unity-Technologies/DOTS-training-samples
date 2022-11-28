using Unity.Entities;
using Unity.Mathematics;

public struct GoalComponent : IComponentData
{
    public float2 position;
    public int team;
}
