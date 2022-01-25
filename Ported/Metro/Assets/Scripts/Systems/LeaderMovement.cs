using Unity.Entities;
using Unity.Transforms;

public partial class LeaderMovement : SystemBase
{

    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;

        Entities.WithAll<Leader>().ForEach((ref Translation translation,
                                            ref Rotation rotation, 
                                            ref TrackProgress trackProgress, 
                                            ref NextStation nextStation, 
                                            ref Ticker ticker, 
                                            in Speed speed) => 
        {

            if (ticker.TimeRemaining <= 0)
            {   
                 trackProgress.Value += speed.Value * deltaTime;
                 if (trackProgress.Value >= nextStation.TrackProgress)
                 {
                     trackProgress.Value = nextStation.TrackProgress;
                     nextStation.TrackProgress += 50;
                     ticker.TimeRemaining += 5;
                 }
                 
                 translation.Value.x = trackProgress.Value;   
            }
            else
            {
                ticker.TimeRemaining -= deltaTime;
            }

        }).ScheduleParallel();
    }
}
