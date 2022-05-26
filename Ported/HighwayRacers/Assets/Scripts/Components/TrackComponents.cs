using Unity.Entities;


struct TrackNeedsGeneration : IComponentData
{
}

struct TrackSectionPrefabs : IComponentData
{
    public Entity LinearPrefab;
    public Entity CurvedPrefab;
}

struct TrackSection : IComponentData
{
}

