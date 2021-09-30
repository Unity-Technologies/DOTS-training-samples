using Unity.Mathematics;
using Unity.Entities;

[GenerateAuthoringComponent]
public struct Constants : IComponentData
{
    public uint MinBeesToSpawnFromFood;
    public uint MaxBeesToSpawnFromFood;
    public float MinBeeVelocity;
    public float MaxBeeVelocity;
    public float MinBeeChangeVelocityTime;
    public float MaxBeeChangeVelocityTime;
}
