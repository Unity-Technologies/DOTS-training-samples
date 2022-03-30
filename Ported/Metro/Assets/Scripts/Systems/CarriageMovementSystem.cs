using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial class CarriageMovementSystem : SystemBase
{
    private int offset = 1;
    
    protected override void OnUpdate()
    {
        var lineFromEntity = GetBufferFromEntity<BezierPointBufferElement>(true);
        
        Entities.WithReadOnly(lineFromEntity).ForEach((ref CarriageComponent carriage, 
            ref Translation translation, ref Rotation rotation) =>
        {
            var trainComponent = GetComponent<TrainComponent>(carriage.Train);
            var line = lineFromEntity[trainComponent.Line];
            var length = GetComponent<LineTotalDistanceComponent>(trainComponent.Line).Value;
            
            float trainPosition = GetComponent<TrackPositionComponent>(carriage.Train).Value;
            trainPosition += carriage.Index * 0.01f; // Update carriage offset with variable stored... elsewhere
            translation.Value = BezierHelpers.GetPosition(line, length, math.frac(trainPosition));
            
            var lookRot = quaternion.LookRotation(
                BezierHelpers.GetNormalAtPosition(line, length, math.frac(trainPosition)), 
                new float3(0, 1, 0));
            rotation = new Rotation { Value = lookRot };
        }).ScheduleParallel();
    }
}