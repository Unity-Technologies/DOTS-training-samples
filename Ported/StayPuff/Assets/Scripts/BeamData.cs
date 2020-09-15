using Unity.Entities;

[GenerateAuthoringComponent]
public struct BeamData : IComponentData
{
    public Entity Prefab;
    public int CountX;
    public int CountZ;
}