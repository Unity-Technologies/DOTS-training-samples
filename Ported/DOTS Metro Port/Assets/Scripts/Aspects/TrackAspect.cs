using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Rendering;

public readonly partial struct TrackAspect : IAspect<TrackAspect>
{
	public readonly Entity Entity;
	public readonly DynamicBuffer<BezierPoint> TrackBuffer;
	public readonly DynamicBuffer<Platform> Platforms;
	public readonly RefRO<URPMaterialPropertyBaseColor> BaseColor;
}
