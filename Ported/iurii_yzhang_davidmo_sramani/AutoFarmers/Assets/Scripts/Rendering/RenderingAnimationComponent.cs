using Unity.Entities;
using Unity.Mathematics;

public struct RenderingAnimationComponent : IComponentData
{
    public float2 currentPosition;
    public float2 targetPosision;
}
