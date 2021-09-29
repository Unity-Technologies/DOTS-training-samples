using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public partial class CarriageMovementSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var splineData = GetSingleton<SplineDataReference>().BlobAssetReference;
        var settings = GetSingleton<Settings>();
        
        Entities.ForEach((ref Translation translation, ref Rotation rotation, in TrainReference trainReference) =>
        {
            TrainMovement trainMovement = GetComponent<TrainMovement>(trainReference.Train);
            LineIndex lineIndex = GetComponent<LineIndex>(trainReference.Train);
            
            float carriageSizeWithMargins = settings.CarriageSizeWithMargins / splineData.Value.splineBlobAssets[lineIndex.Index].length * 1000;

            ref var points = ref splineData.Value.splineBlobAssets[lineIndex.Index].points;
            float offsetPosition = trainMovement.position - trainReference.Index * carriageSizeWithMargins;
            if (offsetPosition < 0)
            {
                var pointsLength = points.Length;
                offsetPosition = pointsLength + offsetPosition - 1;
            }

            (translation.Value, rotation.Value) = TrainMovementSystem.TrackPositionToWorldPosition(
                offsetPosition,
                ref points);
        }).ScheduleParallel();
    }
}
