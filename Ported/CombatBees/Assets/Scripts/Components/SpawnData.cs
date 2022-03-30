using Unity.Entities;

[GenerateAuthoringComponent]
public struct SpawnData : IComponentData
{
    public Entity BeePrefab;
}
