using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct Passenger : IComponentData
{
    public Entity CurrentPlatform;
    public float3 CurrentPlatformPosition;
    public Entity DestinationStation;
    public float WalkSpeed;
}
