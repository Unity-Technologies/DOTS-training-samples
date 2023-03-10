using Unity.Burst;
using Unity.Entities;
using Jobs;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Collections;

[BurstCompile]
[UpdateAfter(typeof(CarSpawnSystem))]
public partial struct CarMoveSystem : ISystem
{


    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Config>();
    }

    public void OnDestroy(ref SystemState state)
    {
     
    }
    
    //[BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        state.Dependency.Complete();
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

        var carquery = SystemAPI.QueryBuilder().WithAll<CarData>().Build();
        var cars = carquery.ToComponentDataArray<CarData>(state.WorldUpdateAllocator);
        cars.SortJob().Schedule().Complete();
        
        var config = SystemAPI.GetSingleton<Config>();
        var testJob = new CarMovementJob
        {
            DeltaTime = SystemAPI.Time.DeltaTime,
            config = config,
            frameCount = UnityEngine.Time.frameCount,
            allCars = cars
        };
        state.Dependency = testJob.ScheduleParallel(state.Dependency);
    }
}