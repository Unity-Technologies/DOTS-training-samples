using Unity.Entities;

// An empty component is called a "tag component".
public struct Blood : IComponentData
{
    public BloodState State;
    public float TimePooling;
}
