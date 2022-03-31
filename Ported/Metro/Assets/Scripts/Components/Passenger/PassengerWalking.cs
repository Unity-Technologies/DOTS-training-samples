using Unity.Entities;
using Unity.Mathematics;

public struct PassengerWalking : IComponentData
{
    public float3 WalkDestination;
    public float WalkSpeed;
}