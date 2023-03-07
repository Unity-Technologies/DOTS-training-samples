using Unity.Entities;

// Enableable component type
public struct OvertakeTimerState : IComponentData, IEnableableComponent
{
    public float Value;
}