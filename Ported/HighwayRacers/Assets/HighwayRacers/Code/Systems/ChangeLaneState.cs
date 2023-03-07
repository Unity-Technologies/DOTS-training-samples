using Unity.Entities;

// Enableable component type
public struct ChangeLaneState : IComponentData, IEnableableComponent
{
    public float Value;
}