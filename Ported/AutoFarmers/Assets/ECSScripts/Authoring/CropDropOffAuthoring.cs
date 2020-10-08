using Unity.Entities;
using Unity.Mathematics;

public struct CropDropOff : IComponentData
{
    public float Completion;
    public float3 FromPosition;
    public float3 ToPosition;
}
