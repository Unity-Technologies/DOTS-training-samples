using Unity.Entities;

// Enableable component type
public struct ChangingLaneState : IComponentData, IEnableableComponent
{
    public float Value;
}