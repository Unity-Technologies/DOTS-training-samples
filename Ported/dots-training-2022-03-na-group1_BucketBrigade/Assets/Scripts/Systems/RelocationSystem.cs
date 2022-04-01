using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

[UpdateAfter(typeof(IdleSystem))]
[UpdateBefore(typeof(FirePropagationSystem))]
public partial class RelocationSystem : SystemBase
{
    public const int ResetAllRelocationCount = 60;
    
    protected override void OnUpdate()
    {
        var deltaTime = (float)Time.DeltaTime;

        if ((BucketBrigadeUtility.GetCurrentFrame() % ResetAllRelocationCount) == 0)
        {
            Entities
                .ForEach((ref MyWorkerState state, ref RelocatePosition destination, in Position movement, in Speed speed ) =>
                {
                    if (state.Value == WorkerState.Repositioning)
                    {
                        destination.Value = movement.Value;
                        state.Value = WorkerState.Idle;
                    }
                }).ScheduleParallel();
        }
        else
        {
            Entities
                .ForEach((ref Position movement, ref MyWorkerState state, in RelocatePosition destination, in Speed speed ) =>
                {
                    if (state.Value == WorkerState.Repositioning)
                    {
                        var movementThisFrame = deltaTime * speed.Value;

                        var remainingDisplacement = destination.Value - movement.Value;

                        var remainingDistance = math.length(remainingDisplacement);

                        if (movementThisFrame > remainingDistance)
                        {
                            movement.Value = destination.Value;
                            state.Value = WorkerState.Idle;
                        }
                        else
                        {
                            var t = movementThisFrame / remainingDistance;
                            movement.Value = movement.Value + new float2(remainingDisplacement.x * t, remainingDisplacement.y * t);
                        }
                    }
                    else if (math.distancesq(movement.Value, destination.Value) > 0.01)
                    {
                        state.Value = WorkerState.Repositioning;
                    }
                }).ScheduleParallel();
        }
    }
}
