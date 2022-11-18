
using Unity.Entities;
public struct CurrentGameState
{
    public int score;
}

public struct GameConfig : IComponentData
{
    public Entity tile;
    public Entity wallPrefab;
    public Entity playerSpawnPrefab;
    public Entity playerPrefab;
    public Entity zombiePrefab;
    public Entity movingWallPrefab;
    public Entity pillPrefab;
    public int numMovingWalls;
    public int movingWallsLength;
    public int movingWallRangeMin;
    public int movingWallRangeMax;
    public int mazeSize;
    public bool parallelMazeGen;
    public int openStripCount;
    public int openStripWidth;
    public int mazeStripWidth;
    public int num_zombies;
    public int numPills;
    public uint seed;

    public CurrentGameState gameState;
}