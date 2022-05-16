using Unity.Entities;

class HighwayConfigAuthoring : UnityEngine.MonoBehaviour
{
    public UnityEngine.GameObject StraightRoadPrefab;
    public UnityEngine.GameObject CurvedRoadPrefab;
    public float InsideLaneLength;
}

class HighwayConfigBaker : Baker<HighwayConfigAuthoring>
{
    public override void Bake(HighwayConfigAuthoring authoring)
    {
        AddComponent(new HighwayConfig
        {
            StraightRoadPrefab = GetEntity(authoring.StraightRoadPrefab),
            CurvedRoadPrefab = GetEntity(authoring.CurvedRoadPrefab),
            InsideLaneLength = authoring.InsideLaneLength
        });
    }
}