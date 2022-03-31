using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct HeldByBeeComponent : IComponentData
{
	public Entity HoldingBee;

    // TODO: Would this be better?
    public float3 BeePosition;
    public float3 BeeVelocity;
    public float BeeSize;
}
