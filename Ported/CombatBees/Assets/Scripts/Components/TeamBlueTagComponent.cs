using Unity.Entities;
using Unity.Mathematics;

public struct TeamBlueTagComponent : IComponentData
{
	public static readonly float4 TeamColor = new float4(0, 0, 1, 1);
}
