using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct SpawnBloodConfig : IComponentData
{
    public float3 SpawnLocation;
    //public float3 SpawnAreaSize;
    public int SplattersCount;
}
