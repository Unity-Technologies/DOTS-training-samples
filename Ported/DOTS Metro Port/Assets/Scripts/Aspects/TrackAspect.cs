using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;

public readonly partial struct TrackAspect : IAspect<TrackAspect>
{
	public readonly Entity Entity;
	public readonly DynamicBuffer<BezierPoint> TrackBuffer;
}
