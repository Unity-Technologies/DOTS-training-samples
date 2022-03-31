using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct Passenger : IComponentData
{
    public float3 FinalDestination;
    public float WalkSpeed;
}
