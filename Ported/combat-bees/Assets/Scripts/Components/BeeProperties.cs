using Unity.Entities;
using Unity.Mathematics;

public struct BeeProperties : IComponentData
{
    public BeeMode BeeMode;

    // Data sheet lists a TargetBee but no TargetFood, using a single target for now.
    public Entity Target;

    public float3 TargetPosition;

    public float Aggressivity;
}

public struct Dead : IComponentData, IEnableableComponent
{
}