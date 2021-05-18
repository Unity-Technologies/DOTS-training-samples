using Unity.Entities;
using Unity.Mathematics;

public struct GameConfig : IComponentData
{
    public float MouseSpeed;
    public float CatSpeed;
    public int RoundDuration;
    public float2 BoardDimensions;

    // TODO: probably lots of other stuff
}
