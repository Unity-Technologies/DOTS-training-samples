using Unity.Entities;

[GenerateAuthoringComponent]
public struct PrefabReferenceComponent : IComponentData
{
    public Entity ArrowPrefab0;
    public Entity ArrowPrefab1;
    public Entity ArrowPrefab2;
    public Entity ArrowPrefab3;
    public Entity PreviewArrowPrefab;
    public Entity CellPrefab;
    public Entity SpawnerPrefab;
}
