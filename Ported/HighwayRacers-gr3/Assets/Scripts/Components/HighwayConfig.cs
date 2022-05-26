using Unity.Entities;

struct HighwayConfig : IComponentData
{
    public Entity StraightRoadPrefab;
    public Entity CurvedRoadPrefab;
    public int InsideLaneLength;
    public float InsideLaneWidth;
    public int LaneCount;
}
