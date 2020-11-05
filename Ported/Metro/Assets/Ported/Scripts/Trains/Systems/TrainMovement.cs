using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace MetroECS.Trains
{
    [UpdateAfter(typeof(TrainGeneration))]
    public class TrainMovement : SystemBase
    {
        protected override void OnUpdate()
        {
            var deltaTime = Time.DeltaTime;

            var trainMovementHandle = Entities.ForEach((ref Train train) =>
            {
                train.Position = (train.Position + (deltaTime * Train.MAX_SPEED)) % 1;
            }).ScheduleParallel(Dependency);

            var carriangeMovementHandle = Entities.ForEach((ref Translation translation, ref Rotation rotation, in Carriage carriage) =>
            {
                var train = GetComponent<Train>(carriage.Train);
                var pathRef = GetComponent<PathDataRef>(train.Path);
                var nativePathData = pathRef.ToNativePathData();

                var carriageOffset = (Carriage.LENGTH / 2) + Carriage.SPACING;
                var carriageTrackPosition = train.Position - (carriage.Index * carriageOffset); 
                    
                var carriageWorldPosition = BezierHelpers.GetPosition(nativePathData.Positions, nativePathData.HandlesIn, nativePathData.HandlesOut,
                    nativePathData.Distances, nativePathData.TotalDistance, carriageTrackPosition);
                var carriangeNormal = BezierHelpers.GetNormalAtPosition(nativePathData.Positions, nativePathData.HandlesIn, nativePathData.HandlesOut,
                    nativePathData.Distances, nativePathData.TotalDistance, carriageTrackPosition);
                
                translation.Value = carriageWorldPosition;
                rotation.Value = quaternion.LookRotation(carriangeNormal, new float3(0, 1, 0));
            }).ScheduleParallel(trainMovementHandle);

            Dependency = carriangeMovementHandle;
        }
    }
}