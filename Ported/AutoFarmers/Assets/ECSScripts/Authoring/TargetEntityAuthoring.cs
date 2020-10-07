using Unity.Entities;
using Unity.Mathematics;

public struct TargetEntity : IComponentData
{
    public Entity target;
    public float2 targetPosition;
}