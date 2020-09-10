using Unity.Entities;

[GenerateAuthoringComponent]
public struct BoardCreationAuthor : IComponentData
{
    public Entity TilePrefab;
    public Entity WallPrefab;
    public Entity GoalPrefab;
    public int SizeY;
    public int SizeX;
    
    public Entity RatSpawner;
    public Entity CatSpawner;
}
