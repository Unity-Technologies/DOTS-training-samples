using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct BoardCreationAuthor : IComponentData
{
    public Entity TilePrefab;
    public Entity WallPrefab;
    public Entity GoalPrefab;
    public int SizeY;
    public int SizeX;
}
