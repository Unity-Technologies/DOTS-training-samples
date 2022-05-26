using Unity.Entities;
using Unity.Mathematics;

public struct TillGroundTarget : IComponentData
{
    public int tileIndex;
    public float2 tileTranslation;
}
