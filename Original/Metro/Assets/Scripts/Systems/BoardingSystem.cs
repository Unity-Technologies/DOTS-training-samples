using dots_src.Components;
using Unity.Collections;
using Unity.Entities;

namespace dots_src.Systems
{
    public partial class BoardingSystem : SystemBase {
        protected override void OnUpdate()
        {
            var deltaTime = Time.DeltaTime;
            var trainStates = GetComponentDataFromEntity<TrainState>();
            
            Entities.WithNativeDisableParallelForRestriction(trainStates)
                .ForEach((ref Occupancy occupancy) =>
                {
                    if (occupancy.Train == Entity.Null) return;

                    if (occupancy.TimeLeft > 0)
                        occupancy.TimeLeft -= deltaTime;
                    else
                    {
                        trainStates[occupancy.Train] = new TrainState {State = TrainMovementStates.Starting};
                        occupancy.Train = Entity.Null;
                    }
                }).ScheduleParallel();
        }
    }
}
