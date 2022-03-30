using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

[UpdateAfter(typeof(IdleSystem))]
[UpdateBefore(typeof(FirePropagationSystem))]
public partial class RelocationSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var deltaTime = (float)Time.DeltaTime;


        Entities
            .ForEach((ref Position movement, ref MyWorkerState state, in Destination destination, in Speed speed ) =>
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
