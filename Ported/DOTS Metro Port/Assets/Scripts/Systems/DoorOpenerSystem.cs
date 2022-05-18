using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Unity.Burst;

[BurstCompile]
public partial struct DoorOpenerSystem : ISystem
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
		var job = new DoorOpeningJob { ElapsedTime = state.Time.ElapsedTime };
		job.ScheduleParallel();
	}

}

public partial struct DoorOpeningJob : IJobEntity
{
	[ReadOnly]
	public BufferFromEntity<BezierPoint> BufferFromEntity;
	public double ElapsedTime;
	public void Execute(in Door door, TransformAspect transform)
	{
		var desiredPosition = math.lerp(door.StartPosition, door.EndPosition, (float)math.abs(math.cos(ElapsedTime)));

		transform.LocalPosition = desiredPosition;
	}
}

