using Unity.Entities;

struct HighwayConfig : IComponentData
{
    public Entity StraightRoadPrefab;
    public Entity CurvedRoadPrefab;
    public int InsideLaneLength;
}
