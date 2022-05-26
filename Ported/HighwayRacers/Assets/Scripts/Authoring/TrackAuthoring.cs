using Unity.Entities;

class TrackAuthoring : UnityEngine.MonoBehaviour
{
    public UnityEngine.GameObject LinearPrefab;
    public UnityEngine.GameObject CurvedPrefab;
}

class TrackBaker : Baker<TrackAuthoring>
{
    public override void Bake(TrackAuthoring authoring)
    {        
        AddComponent<TrackSectionPrefabs>(new TrackSectionPrefabs
        {
            LinearPrefab = GetEntity(authoring.LinearPrefab),
            CurvedPrefab = GetEntity(authoring.CurvedPrefab)
        });
        AddComponent<TrackNeedsGeneration>();
    }
}
