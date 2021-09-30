using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public partial class CarriageMovementSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var settings = GetSingleton<Settings>();
        
        Entities.ForEach((ref Translation translation, ref Rotation rotation, in TrainReference trainReference) =>
        {
            var trainMovement = GetComponent<TrainMovement>(trainReference.Train);
            var lineIndex = GetComponent<LineEntity>(trainReference.Train);
            ref var splineBlobAsset = ref GetComponent<SplineDataReference>(lineIndex.Value).SplineBlobAsset.Value;
            
            var carriageSizeWithMargins = splineBlobAsset.DistanceToPointUnitDistance(settings.CarriageSizeWithMargins);

            var pointUnitPos = trainMovement.position - trainReference.Index * carriageSizeWithMargins;
            if (pointUnitPos < 0) pointUnitPos += splineBlobAsset.equalDistantPoints.Length - 1;

            (translation.Value, rotation.Value) = splineBlobAsset.PointUnitPosToWorldPos(pointUnitPos);
        }).ScheduleParallel();
    }
}
