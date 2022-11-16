
using Unity.Entities;

public struct GameConfig : IComponentData
{
    public Entity tile;
    public Entity wallPrefab;
    public Entity playerSpawnPrefab;
    public Entity playerPrefab;
    public Entity movingWallPrefab;
    public int numMovingWalls;
    public int movingWallsLength;
    public int movingWallRangeMin;
    public int movingWallRangeMax;
    public int mazeSize;
    public int openStripWidth;
    public int mazeStripWidth;
}