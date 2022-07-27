using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
struct RockConfig : IComponentData
{
    public Entity RockPrefab;
    public int NumRocks;
    public float2 RandomSizeMin;
    public float2 RandomSizeMax;
    public float minHeight;
    public float maxHeight;
}