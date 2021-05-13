using Unity.Entities;

public struct BoardPrefab : IComponentData
{
    public Entity LightCellPrefab;
    public Entity DarkCellPrefab;
    public Entity CursorPrefab;
    public Entity PlayerCursorPrefab;
    public Entity ArrowPrefab;
    public Entity WallPrefab;
    public Entity GoalPrefab;
}
