using Unity.Entities;

public struct PrefabConfig : IComponentData
{
    public Entity WallPrefab;
    public Entity PillPrefab;
    public Entity CharacterPrefab;
    public Entity TilePrefab;
    public Entity ZombiePrefab;
    public Entity MovingWallPrefab;
}
