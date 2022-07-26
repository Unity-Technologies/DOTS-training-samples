using Unity.Entities;
using Unity.Mathematics;
using Unity.VisualScripting;

public struct Food : IComponentData
{
    public Entity target;

    public float3 targetPos;
}