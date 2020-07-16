using Unity.Entities;

[GenerateAuthoringComponent]
public struct DespawnTimer : IComponentData
{
    public float Time;
}
