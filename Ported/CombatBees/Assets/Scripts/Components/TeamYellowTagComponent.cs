using Unity.Entities;
using Unity.Mathematics;

public struct TeamYellowTagComponent : IComponentData
{
	public static readonly float4 TeamColor = new float4(1, 1, 0, 1);
}
