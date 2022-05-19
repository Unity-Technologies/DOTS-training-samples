using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using Unity.Transforms;
using Unity.Mathematics;

[BurstCompile]
[UpdateAfter(typeof(TrainStateSystem))]
partial struct TrainMoverSystem : ISystem
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
        Config config = SystemAPI.GetSingleton<Config>();

        var job = new MoveTrainJob { DeltaTime = state.Time.DeltaTime, MaxTrainSpeed = config.MaxTrainSpeed };
        job.ScheduleParallel();
    }

}

[BurstCompile]
public partial struct MoveTrainJob : IJobEntity
{
    public float DeltaTime;
    public float MaxTrainSpeed;

    [BurstCompile]
    public void Execute(ref DistanceAlongBezier trainPosition, in Train train)
	{
        switch (train.TrainState)
        {
            case TrainState.Moving:
            case TrainState.Stopping:
            {
                MoveTrain(ref trainPosition, train, MaxTrainSpeed, DeltaTime);
                break;
            }
        }
    }

    [BurstCompile]
    private void MoveTrain(ref DistanceAlongBezier trainPosition, Train train, float maxTrainSpeed, float deltaTime)
    {
        trainPosition.Distance += maxTrainSpeed * train.SpeedPercentage * deltaTime;
    }
}

