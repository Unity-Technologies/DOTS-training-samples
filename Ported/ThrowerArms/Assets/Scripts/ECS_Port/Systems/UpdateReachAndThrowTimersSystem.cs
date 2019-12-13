using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

[UpdateInGroup(typeof(PresentationSystemGroup))]
public class UpdateReachAndThrowTimersSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        float deltaTime = UnityEngine.Time.deltaTime;
        
        JobHandle incrementReachTimerJob =
            Entities.WithAll<ReachForTargetState>()
                .ForEach((ref ArmComponent arm) =>
                {
                    arm.ReachTimer += deltaTime / ArmConstants.ReachDuration;
                    arm.ReachTimer = math.clamp(arm.ReachTimer, 0f, 1f);
                }).Schedule(inputDeps);
        
        JobHandle decrementReachTimerJob =
            Entities.WithAny<IdleState, HoldingRockState>()
                .ForEach((ref ArmComponent arm) =>
                {
                    arm.ReachTimer -= deltaTime / ArmConstants.ReachDuration;
                    arm.ReachTimer = math.clamp(arm.ReachTimer, 0f, 1f);
                }).Schedule(incrementReachTimerJob);
        
        JobHandle incrementThrowTimerJob =
            Entities.WithAll<ThrowAtState>()
                .ForEach((ref ArmComponent arm) =>
                {
                    arm.ThrowTimer += deltaTime / ArmConstants.ThrowDuration;
                    arm.ThrowTimer = math.clamp(arm.ThrowTimer, 0f, 1f);
                }).Schedule(decrementReachTimerJob);

        return incrementThrowTimerJob;
    }
}