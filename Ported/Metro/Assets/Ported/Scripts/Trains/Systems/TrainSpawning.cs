using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class TrainSpawning : SystemBase
{
    protected override void OnUpdate()
    {
        var carriagePrefab = GetSingleton<MetroData>().CarriagePrefab;
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        Entities.ForEach((in PathRef pathRef) =>
        {
            // Extract required arrays
            var positions = pathRef.Data.Value.Positions.ToNativeArray();
            var handlesIn = pathRef.Data.Value.HandlesIn.ToNativeArray();
            var handlesOut = pathRef.Data.Value.HandlesOut.ToNativeArray();
            var distances = pathRef.Data.Value.Distances.ToNativeArray();
            var totalDistance = pathRef.Data.Value.TotalDistance;
            var numberOfTrains = pathRef.Data.Value.NumberOfTrains;
            var maxCarriages = pathRef.Data.Value.MaxCarriages;
            
            var splitDistance = 1f / numberOfTrains;
            var random = new Random(1234);
            var carriageLengthOnRail = Carriage.LENGTH / totalDistance;
            var carriageSpacingOnRail = Carriage.SPACING / totalDistance;
            var carriageOffset = (carriageLengthOnRail + carriageSpacingOnRail) / 2;

            for (var trainID = 0; trainID < numberOfTrains; trainID++)
            {
                var carriageCount = random.NextInt(maxCarriages / 2, maxCarriages);
                var trainLength = (carriageCount * carriageLengthOnRail) + ((carriageCount - 1) * carriageSpacingOnRail);
                var normalizedTrainPosition = (trainID * splitDistance) + (trainLength / 2);

                for (var carriageID = 0; carriageID < carriageCount; carriageID++)
                {
                    // Carriage position
                    var normalizedCarriagePosition = normalizedTrainPosition - (carriageID * carriageOffset);
                    
                    // World position
                    var position = BezierHelpers.GetPosition(positions, handlesIn, handlesOut, distances, totalDistance,
                        normalizedCarriagePosition);
                    
                    // Normal
                    var normal = BezierHelpers.GetNormalAtPosition(positions, handlesIn, handlesOut, distances,
                        totalDistance,
                        normalizedCarriagePosition);

                    // Generate carriage
                    var carriage = ecb.Instantiate(carriagePrefab);

                    // Position
                    var translation = new Translation {Value = position};
                    ecb.SetComponent(carriage, translation);

                    // Rotation
                    var rotation = new Rotation {Value = quaternion.LookRotation(normal, new float3(0, 1, 0))};
                    ecb.SetComponent(carriage, rotation);   
                }
            }
        }).Run();
        
        ecb.Playback(EntityManager);

        Enabled = false;
    }
}
