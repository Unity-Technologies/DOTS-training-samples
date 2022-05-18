using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Unity.Burst;

[BurstCompile]
public partial struct DoorOpenerSystem : ISystem
{
	private ComponentDataFromEntity<Train> m_trainFromEntity;
	private ComponentDataFromEntity<Carriage> m_CarriageFromEntity;

	public void OnCreate(ref SystemState state)
	{
		m_trainFromEntity = state.GetComponentDataFromEntity<Train>(true);
		m_CarriageFromEntity = state.GetComponentDataFromEntity<Carriage>(true);
	}

	public void OnDestroy(ref SystemState state)
	{
	}

	[BurstCompile]
	public void OnUpdate(ref SystemState state)
	{
		//var job = new DoorOpeningJob { ElapsedTime = state.Time.ElapsedTime };
		//job.ScheduleParallel();
		m_trainFromEntity.Update(ref state);
		m_CarriageFromEntity.Update(ref state);

		var job = new ManageDoors { m_trainFromEntity = m_trainFromEntity, m_CarriageFromEntity = m_CarriageFromEntity, time = state.Time.ElapsedTime};
		job.ScheduleParallel();
	}
}

partial struct ManageDoors : IJobEntity
{
	[ReadOnly] public ComponentDataFromEntity<Train> m_trainFromEntity;
	[ReadOnly] public ComponentDataFromEntity<Carriage> m_CarriageFromEntity;
	public double time;

	void Execute(in Door door, TransformAspect transform)
	{
		Train train = m_trainFromEntity[m_CarriageFromEntity[door.Carriage].Train];
		var trainState = train.TrainState;

		float3 desiredPosition = door.StartPosition;

		if (trainState == TrainState.Stopped)
		{
			desiredPosition = math.lerp(door.StartPosition, door.EndPosition, 1f);
		}
		else
		{
			desiredPosition = math.lerp(door.StartPosition, door.EndPosition, 0f);
		}

		transform.LocalPosition = desiredPosition;
	}
}

