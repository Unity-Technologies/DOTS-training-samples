using Unity.Entities;

[GenerateAuthoringComponent]
public struct GlobalPrefabs : IComponentData
{
    public Entity WorkerPrefab;
}