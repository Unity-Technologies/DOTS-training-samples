using Unity.Entities;
using Unity.Mathematics;

public class TrainSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var deltaTime = Time.DeltaTime;
        
        Entities
            .ForEach((ref NextTrainPosition maxPosition, in Position position, in Rail rail, in NextTrain nextTrain) =>
            {
                var railLength = GetComponent<RailLength>(rail);
                var nextTrainPosition = GetComponent<Position>(nextTrain);
                if (nextTrainPosition < position)
                    nextTrainPosition += railLength;

                maxPosition = nextTrainPosition - 10f;
            }).ScheduleParallel();
        
        Entities
            .ForEach((ref Position position, in Rail rail, in NextTrainPosition nextTrainPosition /*, in Speed speed */) =>
            {
                var railLength = GetComponent<RailLength>(rail);
                var newPosition = position + 20f * deltaTime;
                newPosition = math.min(newPosition, nextTrainPosition);
                newPosition = math.frac(newPosition / railLength) * railLength;
                position = newPosition;
            }).ScheduleParallel();
        
        Entities
            .ForEach((ref DynamicBuffer<BufferCarriage> carriages, in Position position, in Rail rail) =>
            {
                for (int i = 0; i < carriages.Length; i++)
                {
                    var carriagePosition = position - i * (TrainCarriage.CARRIAGE_LENGTH + TrainCarriage.CARRIAGE_SPACING);
                    if (carriagePosition < 0f)
                        carriagePosition += GetComponent<RailLength>(rail);
                    SetComponent<CarriagePosition>(carriages[i], carriagePosition);
                }
            }).Schedule();
    }
}
