using Unity.Entities;
using Unity.Mathematics;

public struct GameConfig : IComponentData
{
    public float MouseSpeed;
    public float CatSpeed;
    public int RoundDuration;
    public int2 BoardDimensions;
    public float4 TileColor1;
    public float4 TileColor2;

    public Entity CellPrefab;


    // TODO: probably lots of other stuff
}
