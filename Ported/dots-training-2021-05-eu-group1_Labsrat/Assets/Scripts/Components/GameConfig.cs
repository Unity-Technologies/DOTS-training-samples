using Unity.Entities;
using Unity.Mathematics;

public struct GameConfig : IComponentData
{
    public float MouseSpeed;
    public float CatSpeed;
    public int NumOfCats;
    public int RoundDuration;
    public int2 BoardDimensions;
    public float4 TileColor1;
    public float4 TileColor2;

    public Entity CellPrefab;
    public Entity WallPrefab;
    public float WallProbability;
    public Entity CatPrefab;


    // TODO: probably lots of other stuff
}
