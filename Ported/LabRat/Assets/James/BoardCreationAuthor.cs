using Unity.Entities;

[GenerateAuthoringComponent]
public struct BoardCreationAuthor : IComponentData
{
    public Entity TilePrefab;
    public Entity WallPrefab;
    public Entity GoalPrefab;
    public int SizeY;
    public int SizeX;
    public float randomAmount;
    
    public Entity RatSpawner;
    public Entity CatSpawner;
}
