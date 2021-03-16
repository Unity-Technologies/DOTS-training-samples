using Unity.Entities;

[GenerateAuthoringComponent]
public struct BeeSpawnConfiguration: IComponentData
{
    public int Count;
}