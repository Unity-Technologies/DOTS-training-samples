using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

[BurstCompile]
[UpdateBefore(typeof(CarDrivingSystem))]
[UpdateAfter(typeof(CarCollisionSystem))]
partial struct CarOvertakingSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var overtakeStateJob = new OvertakeStateJob();
        overtakeStateJob.ScheduleParallel();
    }
}

[BurstCompile]
partial struct OvertakeStateJob : IJobEntity
{
    [BurstCompile]
    void Execute(ref CarOvertakeState overtakeState, ref CarPositionInLane positionInLane, in CarCollision collision)
    {
        //Figure out if we should start changing lane
        if (collision.Front)
        {
            if (!collision.Right)
            {
                overtakeState.ChangingLane = true;
                overtakeState.OriginalLane = positionInLane.LaneIndex;
                positionInLane.LaneIndex++;
            }
            else if (!collision.Left)
            {
                overtakeState.ChangingLane = true;
                overtakeState.OriginalLane = positionInLane.LaneIndex;
                positionInLane.LaneIndex--;
            }
        }
        //Figure out if we're finished changing lane
        if (overtakeState.ChangingLane)
        {
            if (math.abs(positionInLane.LaneIndex - positionInLane.LanePosition) <= math.EPSILON)
            {
                overtakeState.ChangingLane = false;
                positionInLane.LanePosition = positionInLane.LaneIndex;
            }
        }

    }
}