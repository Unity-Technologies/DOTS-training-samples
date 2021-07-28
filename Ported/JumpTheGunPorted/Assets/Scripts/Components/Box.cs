using Unity.Entities;
using Unity.Mathematics;
public struct Box : IComponentData
{
	public const float SPACING = 1;
	public const float Y_OFFSET = 0;
	public const float HEIGHT_MIN = .5f;
	public static readonly float4 MIN_HEIGHT_COLOR = new float4(0, 1, 0, 1);
	public static readonly float4 MAX_HEIGHT_COLOR = new float4(99 / 255f, 47 / 255f, 0 / 255f, 1);

}