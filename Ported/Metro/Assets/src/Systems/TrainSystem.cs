using Unity.Entities;
using Unity.Mathematics;

public class TrainSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var deltaTime = Time.DeltaTime;
        
        Entities
            .ForEach((ref MaxPosition maxPosition, in Position position, in Rail rail, in NextTrain nextTrain) =>
            {
                var railLength = GetComponent<RailLength>(rail);
                var nextTrainPosition = GetComponent<Position>(nextTrain);
                if (nextTrainPosition < position)
                    nextTrainPosition += railLength;

                maxPosition = nextTrainPosition - 10f;
            }).ScheduleParallel();
        
        Entities
            .ForEach((ref Position position, in Rail rail, in MaxPosition maxPosition /*, in Speed speed */) =>
            {
                var railLength = GetComponent<RailLength>(rail);
                var newPosition = position + 20f * deltaTime;
                newPosition = math.min(newPosition, maxPosition);
                newPosition = math.frac(newPosition / railLength) * railLength;
                position = newPosition;
            }).ScheduleParallel();

    }
}
