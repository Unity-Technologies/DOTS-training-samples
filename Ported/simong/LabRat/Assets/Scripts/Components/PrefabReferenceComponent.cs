using Unity.Entities;

[GenerateAuthoringComponent]
public struct PrefabReferenceComponent : IComponentData
{
    public Entity ArrowPrefab;
    public Entity PreviewArrowPrefab;
    public Entity CellPrefab;
    public Entity SpawnerPrefab;
}
