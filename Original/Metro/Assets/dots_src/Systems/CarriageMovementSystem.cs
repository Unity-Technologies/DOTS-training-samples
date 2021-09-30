using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public partial class CarriageMovementSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var splineData = GetSingleton<SplineDataReference>().BlobAssetReference;
        var settings = GetSingleton<Settings>();

        Entities.WithNone<DoorMovement>()
            .ForEach((ref Translation translation, ref Rotation rotation, in TrainReference trainReference, in CarriageIndex index) =>
            {
                var trainPosition = GetComponent<TrainPosition>(trainReference.Train);
                var lineIndex = GetComponent<LineIndex>(trainReference.Train);

                ref var splineBlobAsset = ref splineData.Value.splineBlobAssets[lineIndex.Index];

                var carriageSizeWithMargins = splineBlobAsset.DistanceToPointUnitDistance(settings.CarriageSizeWithMargins);

                var pointUnitPos = trainPosition.position - index.Value * carriageSizeWithMargins;
                if (pointUnitPos < 0) pointUnitPos += splineBlobAsset.equalDistantPoints.Length - 1;

                (translation.Value, rotation.Value) = splineBlobAsset.PointUnitPosToWorldPos(pointUnitPos);
            }).ScheduleParallel();
    }
}
