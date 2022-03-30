using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial class CarriageMovementSystem : SystemBase
{
    private int offset = 1;
    
    protected override void OnUpdate()
    {
        var lineFromEntity = GetBufferFromEntity<BezierPointBufferElement>(true);
        
        Entities.WithReadOnly(lineFromEntity).ForEach((ref CarriageComponent carriage, ref Translation translation) =>
        {
            var trainComponent = GetComponent<TrainComponent>(carriage.Train);
            var line = lineFromEntity[trainComponent.Line];
            var length = GetComponent<LineTotalDistanceComponent>(trainComponent.Line).Value;
            
            float trainPosition = GetComponent<TrackPositionComponent>(carriage.Train).Value;
            trainPosition += carriage.Index * 0.01f;

            var position = BezierHelpers.GetPosition(line, length, trainPosition);
            
            translation.Value = position;
        }).ScheduleParallel();
    }
}