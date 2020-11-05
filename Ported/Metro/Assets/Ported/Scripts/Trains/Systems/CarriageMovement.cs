using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace MetroECS.Trains
{
    [UpdateAfter(typeof(TrainInMotionSystem))]
    public class CarriageMovement : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((ref Translation translation, ref Rotation rotation, in Carriage carriage) =>
            {
                var train = GetComponent<Train>(carriage.Train);
                var pathRef = GetComponent<PathDataRef>(train.Path);
                var nativePathData = pathRef.ToNativePathData();

                var carriageOffset = (Carriage.LENGTH + Carriage.SPACING) / nativePathData.TotalDistance;
                var carriageTrackPosition = train.Position - (carriage.ID * carriageOffset);
                if (carriageTrackPosition < 0) carriageTrackPosition += 1;

                var carriageWorldPosition = BezierHelpers.GetPosition(nativePathData.Positions,
                    nativePathData.HandlesIn, nativePathData.HandlesOut,
                    nativePathData.Distances, nativePathData.TotalDistance, train.Position);
                var carriageWorldTangent =  BezierHelpers.GetNormalAtPosition(nativePathData.Positions,
                    nativePathData.HandlesIn, nativePathData.HandlesOut,
                    nativePathData.Distances, nativePathData.TotalDistance, train.Position);

                // Ensure the carriages are spaced reasonably uniformly
                if (carriage.ID > 0)
                {
                    var trainWorldPosition = carriageWorldPosition;
                    var trainWorldTangent = math.normalize(carriageWorldTangent);

                    var expectedDistance = carriage.ID * (Carriage.LENGTH + Carriage.SPACING);
                    var carriageTrackOffset = Carriage.SPACING / nativePathData.TotalDistance;
                    var damping = 1.0f;

                    for (int i = 0, maxIter = 300; i < maxIter; i++, damping *= 0.99f)
                    {
                        carriageWorldPosition = BezierHelpers.GetPosition(nativePathData.Positions,
                            nativePathData.HandlesIn, nativePathData.HandlesOut,
                            nativePathData.Distances, nativePathData.TotalDistance, carriageTrackPosition);

                        var trainToCarriage = trainWorldPosition - carriageWorldPosition;
                        var carriageDistance = math.length(trainToCarriage);
                        var carriageAlignment = math.abs(math.dot(trainWorldTangent, trainToCarriage / carriageDistance));
                        if (carriageDistance * carriageAlignment > expectedDistance)
                            carriageTrackPosition += damping * carriageTrackOffset;
                        else if (carriageDistance < expectedDistance * carriageAlignment)
                            carriageTrackPosition -= damping * carriageTrackOffset;
                        else
                            break;
                    }

                    carriageWorldTangent = BezierHelpers.GetNormalAtPosition(nativePathData.Positions,
                        nativePathData.HandlesIn, nativePathData.HandlesOut,
                        nativePathData.Distances, nativePathData.TotalDistance, carriageTrackPosition);
                }

                translation.Value = carriageWorldPosition;
                rotation.Value = quaternion.LookRotation(carriageWorldTangent, new float3(0, 1, 0));
            }).ScheduleParallel();
        }
    }
}