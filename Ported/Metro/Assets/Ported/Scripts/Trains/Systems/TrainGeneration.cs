using MetroECS.Trains;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class TrainGeneration : SystemBase
{
    protected override void OnUpdate()
    {
        var carriagePrefab = GetSingleton<MetroData>().CarriagePrefab;
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        Entities.ForEach((in Entity pathDataEntity, in PathDataRef pathRef) =>
        {
            var nativePathData = pathRef.ToNativePathData();

            var splitDistance = 1f / nativePathData.NumberOfTrains;
            var random = new Random(1234);
            var carriageLengthOnRail = Carriage.LENGTH / nativePathData.TotalDistance;
            var carriageSpacingOnRail = Carriage.SPACING / nativePathData.TotalDistance;
            var carriageOffset = (carriageLengthOnRail + carriageSpacingOnRail) / 2;

            for (var trainID = 0; trainID < nativePathData.NumberOfTrains; trainID++)
            {
                var carriageCount = random.NextInt(nativePathData.MaxCarriages / 2, nativePathData.MaxCarriages);
                var trainLength = (carriageCount * carriageLengthOnRail) + ((carriageCount - 1) * carriageSpacingOnRail);
                var normalizedTrainPosition = (trainID * splitDistance) + (trainLength / 2);

                // Create train
                var trainEntity = ecb.CreateEntity();
                var trainData = new Train
                    {Position = normalizedTrainPosition, CarriageCount = carriageCount, Path = pathDataEntity};
                ecb.AddComponent(trainEntity, trainData);
                
                for (var carriageID = 0; carriageID < carriageCount; carriageID++)
                {
                    // Carriage position
                    var normalizedCarriagePosition = normalizedTrainPosition - (carriageID * carriageOffset);
                    
                    // World position
                    var position = BezierHelpers.GetPosition(nativePathData.Positions, nativePathData.HandlesIn, nativePathData.HandlesOut, nativePathData.Distances, nativePathData.TotalDistance,
                        normalizedCarriagePosition);
                    
                    // Normal
                    var normal = BezierHelpers.GetNormalAtPosition(nativePathData.Positions, nativePathData.HandlesIn, nativePathData.HandlesOut, nativePathData.Distances,
                        nativePathData.TotalDistance,
                        normalizedCarriagePosition);

                    // Generate carriage
                    var carriageEntity = ecb.Instantiate(carriagePrefab);
                    var carriageData = new Carriage {Index = carriageID, Train = trainEntity};
                    ecb.SetComponent(carriageEntity, carriageData);

                    // World position
                    var translation = new Translation {Value = position};
                    ecb.SetComponent(carriageEntity, translation);

                    // Rotation
                    var rotation = new Rotation {Value = quaternion.LookRotation(normal, new float3(0, 1, 0))};
                    ecb.SetComponent(carriageEntity, rotation);
                }
            }
        }).Run();
        
        ecb.Playback(EntityManager);

        Enabled = false;
    }
}
