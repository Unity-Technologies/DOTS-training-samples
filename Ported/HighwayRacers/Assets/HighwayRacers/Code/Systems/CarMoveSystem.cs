using Unity.Burst;
using Unity.Entities;
using Jobs;
using Unity.Jobs;
using UnityEngine.Profiling;

[BurstCompile]
public partial struct CarMoveSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Config>();
    }

    public void OnDestroy(ref SystemState state)
    {
     
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        Profiler.BeginSample("Car Move System Update**********************");

        if (state.Dependency != null)
        {
            Profiler.BeginSample("Waiting for previous frame jobs");
            state.Dependency.Complete();
            Profiler.EndSample();
        }

        /*
	       	CarMoveSystem
			Query for all cars.
			If car in front (tailgating)
				ENABLE 'ChangeLaneState' with desired Lane if not already tagged
				ENABLE 'OvertakeTimerState'
			Else (no car in front)
				DISABLE 'ChangeLaneState'
				Move cars forward at desired speed.
					If 'OvertakeTimerState' ENABLED
						Move at Overtake speed
					Else
						Move at normal speed
         */
        
        var config = SystemAPI.GetSingleton<Config>();
        var testJob = new CarMovementJob
        {
            DeltaTime = SystemAPI.Time.DeltaTime
        };
        JobHandle jobHandle = testJob.ScheduleParallel(state.Dependency);
        state.Dependency = jobHandle;        

        Profiler.EndSample();
    }
}