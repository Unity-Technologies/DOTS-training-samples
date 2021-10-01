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
            var platformRefs = GetComponentDataFromEntity<PlatformRef>();
            
            Entities.WithNativeDisableParallelForRestriction(trainStates)
                .ForEach((ref Occupancy occupancy, ref BoardingState _boardingState) =>
                {
                    if (occupancy.Train == Entity.Null) return;

                    if (occupancy.TimeLeft > 0)
                    {
                        _boardingState.State = BoardingStates.Arriving;
                        occupancy.TimeLeft -= deltaTime;
                    }
                    else
                    {
                        trainStates[occupancy.Train] = new TrainState {State = TrainMovementStates.Starting};
                        //platformRefs[occupancy.Train] = new PlatformRef();
                        occupancy.Train = Entity.Null;
                    }
                    
                    if (occupancy.TimeLeft < .2f)
                    {
                        _boardingState.State = BoardingStates.Leaving;
                    }
                }).ScheduleParallel();

            Entities
                .WithNativeDisableParallelForRestriction(platformRefs)
                .WithAll<CarriageIndex>().ForEach((Entity e, in TrainReference trainReference) =>
                {
                    platformRefs[e] = platformRefs[trainReference.Train];
                }).ScheduleParallel();
        }
    }
}
