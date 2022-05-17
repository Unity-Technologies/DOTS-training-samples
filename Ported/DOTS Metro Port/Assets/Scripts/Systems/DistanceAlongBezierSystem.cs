using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Unity.Burst;

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
		var job = new PositionEntityOnBezierJob{ BufferFromEntity = _bufferFromEntity };
		job.ScheduleParallel();

		//foreach (var (position, transform) in SystemAPI.Query<RefRO<DistanceAlongBezier>, TransformAspect>())
		//{
		//	DynamicBuffer<BezierPoint> track = _bufferFromEntity[position.ValueRO.TrackEntity];
		//	PositionEntityOnBezier(position, transform, track.AsNativeArray());
		//}
	}

	public static void PositionEntityOnBezier(in DistanceAlongBezier position, TransformAspect transform, NativeArray<BezierPoint> track)
	{
		float pathLength = BezierPath.Get_PathLength(track);
		float distAsPercentage = position.Distance / pathLength;
		float3 posOnRail = BezierPath.Get_Position(track, distAsPercentage);
		float3 tangentOnRail = BezierPath.Get_NormalAtPosition(track, distAsPercentage);
		
		var rotation = Quaternion.LookRotation(tangentOnRail);
		transform.Position = posOnRail;
		transform.Rotation = rotation;
	}
}

public partial struct PositionEntityOnBezierJob : IJobEntity
{
	[ReadOnly]
	public BufferFromEntity<BezierPoint> BufferFromEntity;
	public void Execute(in DistanceAlongBezier position, TransformAspect transform)
	{
		//DynamicBuffer<BezierPoint> track = SystemAPI.GetBuffer<BezierPoint>(position.TrackEntity);
		DynamicBuffer<BezierPoint> track = BufferFromEntity[position.TrackEntity];
		DistanceAlongBezierSystem.PositionEntityOnBezier(position, transform, track.AsNativeArray());
	}
}
