using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Unity.Burst;
using Unity.Jobs;

[BurstCompile]
public partial struct DistanceAlongBezierSystem : ISystem
{
	private BufferFromEntity<BezierPoint> _bufferFromEntity;

	public void OnCreate(ref SystemState state)
	{
		_bufferFromEntity = state.GetBufferFromEntity<BezierPoint>(true);
	}

	public void OnDestroy(ref SystemState state)
	{
	}

	[BurstCompile]
	public void OnUpdate(ref SystemState state)
	{
		_bufferFromEntity.Update(ref state);
		var copyBufferJob = new CopyBufferDataJob();
		var positionEntityJob = new PositionEntityOnBezierJob{ BufferFromEntity = _bufferFromEntity };

		copyBufferJob.ScheduleParallel();
		positionEntityJob.ScheduleParallel();
	}
}

[BurstCompile]
public partial struct PositionEntityOnBezierJob : IJobEntity
{
	[ReadOnly]
	public BufferFromEntity<BezierPoint> BufferFromEntity;

	[BurstCompile]
	public void Execute(ref DistanceAlongBezier position, TransformAspect transform)
	{
		DynamicBuffer<BezierPoint> track = BufferFromEntity[position.TrackEntity];
		PositionEntityOnBezier(ref position, ref transform, track.AsNativeArray());
	}

	[BurstCompile]
	public void PositionEntityOnBezier(ref DistanceAlongBezier position, ref TransformAspect transform, in NativeArray<BezierPoint> track)
	{
		float pathLength = BezierPath.Get_PathLength(track);
		position.Distance = Mathf.Repeat(position.Distance, pathLength);
		float distAsPercentage = position.Distance / pathLength;
		float3 posOnRail = BezierPath.Get_Position(track, distAsPercentage);
		float3 tangentOnRail = BezierPath.Get_NormalAtPosition(track, distAsPercentage);

		var rotation = Quaternion.LookRotation(tangentOnRail);
		transform.Position = posOnRail;
		transform.Rotation = rotation;
	}
}


[BurstCompile]
public partial struct CopyBufferDataJob : IJobEntity
{
	[BurstCompile]
	public void Execute(ref DistanceAlongBezier distance, in DistanceAlongBezierBuffer distanceBuffer)
	{
		distance.Distance = distanceBuffer.Value;
	}
}