using Unity.Entities;
using Unity.Mathematics;

public struct Constraint : IBufferElementData
{
    public Entity pointA;
    public Entity pointB;
    public float distance;

	public void AssignPoints(Entity a, Entity b, float3 positionA, float3 positionB)
	{
		pointA = a;
		pointB = b;

		float3 delta = positionB - positionA;
		distance = math.length(delta);

		//thickness = Random.Range(.25f, .35f);
	}
}