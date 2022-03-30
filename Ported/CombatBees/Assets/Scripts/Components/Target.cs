using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct Target : IComponentData
{
    public enum TargetType : uint
    {
        Enemy,
        Resource,
        Goal
    };

    // Generic entity ID for now to keep data uniform. Can be reviewed later.
    public Entity TargetEntity;
    public float3 Position;
    public TargetType Type;
}
