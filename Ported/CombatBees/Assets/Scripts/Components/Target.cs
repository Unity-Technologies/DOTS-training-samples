using System.Diagnostics.Tracing;
using Unity.Entities;
using Unity.Mathematics;

public enum TargetType
{
    None,
    Food,
    Bee,
    Hive,
    Wander,
};

[GenerateAuthoringComponent]
public struct Target : IComponentData
{
    public TargetType TargetType;
    public Entity TargetEntity;
    public float3 TargetPosition;

    public void Reset()
    {
        TargetType = TargetType.None;
        TargetEntity = Entity.Null;
        TargetPosition = float3.zero;
    }
}

