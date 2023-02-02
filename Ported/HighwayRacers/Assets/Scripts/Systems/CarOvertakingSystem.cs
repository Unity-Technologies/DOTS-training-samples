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
        var overtakeStateJob = new OvertakeStateJob
        {
            MergeDirectionIsRight = mergeDirectionIsRight,
            CurrentTime = SystemAPI.Time.ElapsedTime
        };
        overtakeStateJob.ScheduleParallel();
    }
}

[BurstCompile]
partial struct OvertakeStateJob : IJobEntity
{
    public bool MergeDirectionIsRight;
    public double CurrentTime;
    
    [BurstCompile]
    void Execute(ref CarOvertakeState overtakeState, ref CarPositionInLane positionInLane, in CarCollision collision)
    {
        if (overtakeState.IsOvertaking)
        {
            if (CurrentTime - overtakeState.OvertakeStartTime > 3)
            {
                overtakeState.IsOvertaking = false;
                int mergeDirection = positionInLane.LaneIndex - overtakeState.OriginalLane;
                bool canMerge = true;
                if (mergeDirection == 1 && (collision.CollisionFlags & CollisionType.Left) == CollisionType.Left)
                {
                    canMerge = false;
                }
                else if (mergeDirection == -1 && (collision.CollisionFlags & CollisionType.Right) == CollisionType.Right)
                {
                    canMerge = false;
                }

                if (canMerge)
                {
                    int originalLane = overtakeState.OriginalLane;
                    overtakeState.OriginalLane = positionInLane.LaneIndex;
                    positionInLane.LaneIndex = originalLane;
                    overtakeState.ChangingLane = true;
                }
            }
            
        }
        
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
        else if ((collision.CollisionFlags & CollisionType.Front) == CollisionType.Front)
        {
            if (MergeDirectionIsRight && (collision.CollisionFlags & CollisionType.Right) == 0)
            {
                overtakeState.OvertakeStartTime = CurrentTime;
                overtakeState.IsOvertaking = true;
                overtakeState.ChangingLane = true;
                overtakeState.OriginalLane = positionInLane.LaneIndex;
                positionInLane.LaneIndex++;
            }
            else if (!MergeDirectionIsRight && (collision.CollisionFlags & CollisionType.Left) == 0)
            {
                overtakeState.OvertakeStartTime = CurrentTime;
                overtakeState.IsOvertaking = true;
                overtakeState.ChangingLane = true;
                overtakeState.OriginalLane = positionInLane.LaneIndex;
                positionInLane.LaneIndex--;
            }
        }


    }
}