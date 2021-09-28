using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial class CarriageMovementSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var splineData = GetSingleton<SplineDataReference>().BlobAssetReference;
        Entities.ForEach((ref Translation translation, ref Rotation rotation, in TrainReference trainReference) =>
        {
            float carriageSizeWithMargins = 0.2f;
            
            TrainMovement trainMovement = EntityManager.GetComponentData<TrainMovement>(trainReference.Train);
            LineIndex lineIndex = EntityManager.GetComponentData<LineIndex>(trainReference.Train);

            float offsetPosition = trainMovement.position - trainReference.Index * carriageSizeWithMargins;
            if (offsetPosition < 0)
            {
                var pointsLength = splineData.Value.splineBlobAssets[lineIndex.Index].points.Length;
                offsetPosition = pointsLength + offsetPosition - 1;
            }

            (translation.Value, rotation.Value) = TrainMovementSystem.TrackPositionToWorldPosition(
                offsetPosition,
                ref splineData.Value.splineBlobAssets[lineIndex.Index].points);
        }).WithoutBurst().Run(); // TODO: `.WithoutBurst().Run()` likely makes this slower, can we use `.ScheduleParallel()`? 
    }
}
