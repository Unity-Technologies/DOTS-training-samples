

using Unity.Entities;
using Unity.Mathematics;

public struct Team : IComponentData
{
	public int Id;
	public int Length;
	
	public Entity LineFullHead;
	public Entity LineFullTail;
	
	public Entity LineEmptyHead;
	public Entity LineEmptyTail;

	public float3 DropOffLocation;
	public float3 PickupLocation;
}
