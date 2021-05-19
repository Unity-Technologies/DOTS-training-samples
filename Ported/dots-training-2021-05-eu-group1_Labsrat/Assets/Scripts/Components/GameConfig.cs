using Unity.Entities;
using Unity.Mathematics;

public struct GameConfig : IComponentData
{
    public float MouseSpeed;
    public float CatSpeed;
    public int NumOfCats;
    public int NumOfMice;
    public int NumOfAIPlayers;
    public int RoundDuration;
    public int2 BoardDimensions;
    public float SnapDistance;
    public float4 TileColor1;
    public float4 TileColor2;

    public Entity CellPrefab;
    public Entity WallPrefab;
    public float WallProbability;
    public Entity CatPrefab;
    public Entity ArrowPrefab;
    public Entity MousePrefab;

    public bool RandomSeed;
    public uint Seed;
}
