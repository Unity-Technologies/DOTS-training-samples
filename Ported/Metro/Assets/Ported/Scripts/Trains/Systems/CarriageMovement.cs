using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace MetroECS.Trains
{
    [UpdateAfter(typeof(TrainInMotionSystem))]
    [UpdateAfter(typeof(TrainWaitingSystem))]
    public class CarriageMovement : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((ref Translation translation, ref Rotation rotation, ref Carriage carriage) =>
            {
                var train = GetComponent<Train>(carriage.Train);
                var pathRef = GetComponent<PathDataRef>(train.Path);
                var nativePathData = pathRef.ToNativePathData();

                // Are we exactly at the train position (only happens when spawning trains)?
                var spawning = (train.Position == carriage.Position + train.deltaPos);

                float carriageTrackPosition;
                if (spawning)
                {
                    var carriageOffset = (Carriage.LENGTH + Carriage.SPACING) / nativePathData.TotalDistance;
                    carriageTrackPosition = train.Position - (carriage.ID * carriageOffset);
                    if (carriageTrackPosition < 0.0f)
                        carriageTrackPosition += 1.0f;
                }
                else
                {
                    carriageTrackPosition = carriage.Position + train.deltaPos;
                    if (carriageTrackPosition > 1.0f)
                        carriageTrackPosition -= 1.0f;                    
                }

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

                    for (int i = 0, maxIter = 100; i < maxIter; i++, damping *= 0.95f)
                    {
                        carriageWorldPosition = BezierHelpers.GetPosition(nativePathData.Positions,
                            nativePathData.HandlesIn, nativePathData.HandlesOut,
                            nativePathData.Distances, nativePathData.TotalDistance, carriageTrackPosition);

                        var carriageToTrain = trainWorldPosition - carriageWorldPosition;
                        var carriageDistance = math.length(carriageToTrain);
                        var carriageAlignment = math.dot(trainWorldTangent, carriageToTrain / carriageDistance);

                        if (carriageDistance * carriageAlignment > expectedDistance)
                        {
                            carriageTrackPosition += damping * carriageTrackOffset;
                            if (carriageTrackPosition > 1.0f)
                                carriageTrackPosition -= 1.0f;
                        }
                        else if (carriageDistance < expectedDistance * carriageAlignment)
                        {
                            carriageTrackPosition -= damping * carriageTrackOffset;
                            if (!spawning)
                                carriageTrackPosition = math.max(carriageTrackPosition, carriage.Position);
                            if (carriageTrackPosition < 0.0f)
                                carriageTrackPosition += 1.0f;
                        }
                        else
                            break;
                    }

                    carriageWorldTangent = BezierHelpers.GetNormalAtPosition(nativePathData.Positions,
                        nativePathData.HandlesIn, nativePathData.HandlesOut,
                        nativePathData.Distances, nativePathData.TotalDistance, carriageTrackPosition);
                }

                carriage.Position = carriageTrackPosition;

                translation.Value = carriageWorldPosition;
                rotation.Value = quaternion.LookRotation(carriageWorldTangent, UnityEngine.Vector3.up);
            }).ScheduleParallel();
        }
    }
}