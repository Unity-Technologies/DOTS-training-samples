using Unity.Entities;

[GenerateAuthoringComponent]
public struct TrackSpawner : IComponentData
{
    public Entity TrackPrefab;
}