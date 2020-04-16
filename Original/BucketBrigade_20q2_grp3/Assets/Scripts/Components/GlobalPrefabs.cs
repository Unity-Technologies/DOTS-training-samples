using Unity.Entities;

[GenerateAuthoringComponent]
public struct GlobalPrefabs : IComponentData
{
    public Entity WorkerPrefab;
    public Entity BucketPrefab;
}