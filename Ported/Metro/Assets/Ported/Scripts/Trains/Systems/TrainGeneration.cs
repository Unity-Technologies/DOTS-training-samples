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

        Entities.ForEach((in PathDataRef pathRef) =>
        {
            var pathData = pathRef.ToNativePathData();

            var splitDistance = 1f / pathData.NumberOfTrains;
            var random = new Random(1234);
            var carriageLengthOnRail = Carriage.LENGTH / pathData.TotalDistance;
            var carriageSpacingOnRail = Carriage.SPACING / pathData.TotalDistance;
            var carriageOffset = (carriageLengthOnRail + carriageSpacingOnRail) / 2;

            for (var trainID = 0; trainID < pathData.NumberOfTrains; trainID++)
            {
                var carriageCount = random.NextInt(pathData.MaxCarriages / 2, pathData.MaxCarriages);
                var trainLength = (carriageCount * carriageLengthOnRail) + ((carriageCount - 1) * carriageSpacingOnRail);
                var normalizedTrainPosition = (trainID * splitDistance) + (trainLength / 2);

                for (var carriageID = 0; carriageID < carriageCount; carriageID++)
                {
                    // Carriage position
                    var normalizedCarriagePosition = normalizedTrainPosition - (carriageID * carriageOffset);
                    
                    // World position
                    var position = BezierHelpers.GetPosition(pathData.Positions, pathData.HandlesIn, pathData.HandlesOut, pathData.Distances, pathData.TotalDistance,
                        normalizedCarriagePosition);
                    
                    // Normal
                    var normal = BezierHelpers.GetNormalAtPosition(pathData.Positions, pathData.HandlesIn, pathData.HandlesOut, pathData.Distances,
                        pathData.TotalDistance,
                        normalizedCarriagePosition);

                    // Generate carriage
                    var carriageEntity = ecb.Instantiate(carriagePrefab);

                    // var carriage = new Carriage { Index }
                    
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
