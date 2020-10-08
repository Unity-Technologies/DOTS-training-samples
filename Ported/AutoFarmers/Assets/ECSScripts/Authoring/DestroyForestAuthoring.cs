using Unity.Entities;
using Unity.Mathematics;

public struct DestroyForest : IComponentData
{
    public float Completion;
    public float3 OriginalScale;
    public Entity DisplayParentEntity;
}
