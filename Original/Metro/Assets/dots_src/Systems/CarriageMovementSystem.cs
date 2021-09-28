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
        Entities.ForEach((ref Translation translation, ref Rotation rotation, in TrainReference TrainReference) =>
        {
            float carriageSizeWithMargins = 0.2f;
            
            TrainMovement trainMovement = EntityManager.GetComponentData<TrainMovement>(TrainReference.Train);
            LineIndex lineIndex = EntityManager.GetComponentData<LineIndex>(TrainReference.Train);
            float offsetPosition = trainMovement.position - TrainReference.Index * carriageSizeWithMargins;
            if (offsetPosition < 0)
            {
                var pointsLength = splineData.Value.splineBlobAssets[lineIndex.Index].points.Length;
                offsetPosition = pointsLength + offsetPosition - 1;
            }
            (float3 lerpedPosition, Quaternion newRotation) = TrainMovementSystem.TrackPositionToWorldPosition(offsetPosition, ref splineData.Value.splineBlobAssets[lineIndex.Index].points);
            translation.Value = lerpedPosition;
            rotation.Value = newRotation;
        }).WithoutBurst().Run();
    }
}
