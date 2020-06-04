using Unity.Entities;

[GenerateAuthoringComponent]
public struct PrefabReferenceComponent : IComponentData
{
    public Entity ArrowPrefab0;
    public Entity ArrowPrefab1;
    public Entity ArrowPrefab2;
    public Entity ArrowPrefab3;
    
    public Entity BasePrefab0;
    public Entity BasePrefab1;
    public Entity BasePrefab2;
    public Entity BasePrefab3;
    
    public Entity PreviewArrowPrefab;
    public Entity CellPrefab;
    public Entity CatSpawnerPrefab;
    public Entity MouseSpawnerPrefab;
}
