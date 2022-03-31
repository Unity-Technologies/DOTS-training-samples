using Unity.Entities;
using Unity.Mathematics;

public struct HeldByBeeComponent : IComponentData
{
	public Entity HoldingBee;

    // TODO: Would this be better?
    public float3 BeePosition;
    public float3 BeeVelocity;
    public float BeeSize;
}
