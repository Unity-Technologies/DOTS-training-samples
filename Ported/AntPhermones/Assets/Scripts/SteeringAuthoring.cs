using Unity.Entities;

[GenerateAuthoringComponent]
public struct SteeringComponent : IComponentData
{
    public float DesiredYaw;
}
