
using Unity.Entities;

[GenerateAuthoringComponent]
public struct CarSpawnRequest : IComponentData
{
    public int InstancesToSpawn;
}