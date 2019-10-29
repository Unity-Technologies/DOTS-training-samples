using Unity.Entities;

/// <summary>
/// Tag to add one point when a Rat reach a home base
/// </summary>
public struct LbRatScore : IComponentData
{
    public byte Player;
}

/// <summary>
/// Tag to remove 1/3 points from a player when a Cat reach a home base
/// </summary>
public struct LbCatScore : IComponentData
{
    public byte Player;
}