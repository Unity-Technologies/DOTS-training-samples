using Unity.Entities;
using Unity.Mathematics;

public struct ChoppingTask : IComponentData
{
    public Entity Target;
    public float Completion;
    public float3 OriginalScale;
}
