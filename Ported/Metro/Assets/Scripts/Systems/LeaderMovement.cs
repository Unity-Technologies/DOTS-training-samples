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
                                            in Leader leader,
                                            in Speed speed) => 
        {

            if (ticker.TimeRemaining <= 0)
            {   
                 trackProgress.Value += speed.Value * deltaTime;
                 
                 if (trackProgress.Value >= nextStation.TrackProgress)
                 {
                     nextStation.TrackProgress += 20;
                     ticker.TimeRemaining += 5;
                 }
                 
                 var trackData = GetComponent<TrackSimulatedData>(leader.TrackData);
                 translation.Value = trackData.GetPositionForProgress(trackProgress.Value);
                 rotation.Value = trackData.GetQuaternionForProgress(trackProgress.Value);
                 if (trackProgress.Value >= trackData.TotalLength)
                 {
                     trackProgress.Value = 0;
                 }
            }
            else
            {
                ticker.TimeRemaining -= deltaTime;
            }

        }).ScheduleParallel();
    }
}
