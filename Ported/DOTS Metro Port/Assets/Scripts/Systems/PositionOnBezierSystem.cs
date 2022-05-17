using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Unity.Burst;

[BurstCompile]
public partial struct PositionOnBezierSystem : ISystem
{
	public void OnCreate(ref SystemState state)
	{
	}

	public void OnDestroy(ref SystemState state)
	{
	}

	[BurstCompile]
	public void OnUpdate(ref SystemState state)
	{
		var tracks = SystemAPI.Query<DynamicBuffer<BezierPoint>>();
		int trackIndex = 0;
		foreach(var track in tracks)
		{
			foreach (var (position, transform) in SystemAPI.Query<RefRO<PositionOnBezier>, TransformAspect>())
			{
				if(trackIndex == position.ValueRO.BezierIndex)
				{
					PositionEntityOnBezier(position, transform, track.AsNativeArray());
				}
				trackIndex++;
			}
		}
	}

	private void PositionEntityOnBezier(RefRO<PositionOnBezier> position, TransformAspect transform, NativeArray<BezierPoint> track)
	{
		float3 posOnRail = BezierPath.Get_Position(track, position.ValueRO.Position);
		float3 tangentOnRail = BezierPath.Get_NormalAtPosition(track, position.ValueRO.Position);
		
		var rotation = Quaternion.LookRotation(tangentOnRail);
		transform.Position = posOnRail;
		transform.Rotation = rotation;
	}
}
