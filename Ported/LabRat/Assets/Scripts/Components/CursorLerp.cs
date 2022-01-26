using Unity.Entities;
using Unity.Mathematics;

public struct CursorLerp : IComponentData
{
    public float2 Start;
    public float2 Destination;
    public float LerpValue;
}
