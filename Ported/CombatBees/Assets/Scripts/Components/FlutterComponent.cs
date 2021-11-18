using Unity.Entities;
using Unity.Mathematics;


public struct Flutter : IComponentData
{
    public float3 prevValue;
    public float3 nextValue;
    public float3 t;
    public float localSpeed;
    public bool initialized;
}