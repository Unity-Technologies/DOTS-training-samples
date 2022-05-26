using Unity.Entities;

[GenerateAuthoringComponent]
public struct CurrentSpeedComponent : IComponentData
{
    public float CurrentSpeed;
}
