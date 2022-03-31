using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

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
            var carriageLength = GetComponent<LineComponent>(trainComponent.Line).CarriageLength;
            var line = lineFromEntity[trainComponent.Line];
            var lineLength = GetComponent<LineTotalDistanceComponent>(trainComponent.Line).Value;
            
            float offset = carriageLength / lineLength / 2;
            float carriagePosition = math.frac(
                GetComponent<TrackPositionComponent>(carriage.Train).Value + carriage.Index * offset);
            translation.Value = BezierHelpers.GetPosition(line, lineLength, carriagePosition);
            
            var lookRot = quaternion.LookRotation(
                BezierHelpers.GetNormalAtPosition(line, lineLength, carriagePosition), 
                new float3(0, 1, 0));
            rotation = new Rotation { Value = lookRot };
        }).ScheduleParallel();
    }
}