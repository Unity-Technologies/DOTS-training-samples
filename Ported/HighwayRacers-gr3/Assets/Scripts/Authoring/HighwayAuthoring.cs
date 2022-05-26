using Unity.Entities;

class HighwayConfigAuthoring : UnityEngine.MonoBehaviour
{
    public UnityEngine.GameObject StraightRoadPrefab;
    public UnityEngine.GameObject CurvedRoadPrefab;
    public int InsideLaneLength;
    public float InsideLaneWidth;
    public int LaneCount;
}

class HighwayConfigBaker : Baker<HighwayConfigAuthoring>
{
    public override void Bake(HighwayConfigAuthoring authoring)
    {
        AddComponent(new HighwayConfig
        {
            StraightRoadPrefab = GetEntity(authoring.StraightRoadPrefab),
            CurvedRoadPrefab = GetEntity(authoring.CurvedRoadPrefab),
            InsideLaneLength = authoring.InsideLaneLength,
            InsideLaneWidth = authoring.InsideLaneWidth,
            LaneCount = authoring.LaneCount            
        });
    }
}