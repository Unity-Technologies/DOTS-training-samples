using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

[BurstCompile]
[UpdateBefore(typeof(CarDrivingSystem))]
[UpdateAfter(typeof(CarCollisionSystem))]
partial struct CarOvertakingSystem : ISystem
{
    private bool mergeDirectionIsRight;
    
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
        mergeDirectionIsRight = !mergeDirectionIsRight;
        var overtakeStateJob = new OvertakeStateJob{MergeDirectionIsRight = mergeDirectionIsRight};
        overtakeStateJob.ScheduleParallel();
    }
}

[BurstCompile]
partial struct OvertakeStateJob : IJobEntity
{
    public bool MergeDirectionIsRight;
    
    [BurstCompile]
    void Execute(ref CarOvertakeState overtakeState, ref CarPositionInLane positionInLane, in CarCollision collision)
    {
        
        //Figure out if we're finished changing lane
        if (overtakeState.ChangingLane)
        {
            int sign = positionInLane.LaneIndex - overtakeState.OriginalLane;
            if ((positionInLane.LaneIndex - positionInLane.LanePosition) * sign < 0)
            {
                overtakeState.ChangingLane = false;
                positionInLane.LanePosition = positionInLane.LaneIndex;
            }
        }
        //Figure out if we should start changing lane
        else if (collision.Front)
        {
            if (MergeDirectionIsRight && !collision.Right)
            {
                overtakeState.ChangingLane = true;
                overtakeState.OriginalLane = positionInLane.LaneIndex;
                positionInLane.LaneIndex++;
            }
            else if (!MergeDirectionIsRight && !collision.Left)
            {
                overtakeState.ChangingLane = true;
                overtakeState.OriginalLane = positionInLane.LaneIndex;
                positionInLane.LaneIndex--;
            }
        }


    }
}