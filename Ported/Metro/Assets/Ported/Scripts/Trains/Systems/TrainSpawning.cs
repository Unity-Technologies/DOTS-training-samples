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
            var numberOfTrains = pathRef.Data.Value.NumberOfTrains;
            var splitDistance = 1f / numberOfTrains;
             for (var trainID = 0; trainID < numberOfTrains; trainID++)
             {
                 // Extract required arrays
                 var positions = pathRef.Data.Value.Positions.ToNativeArray();
                 var handlesIn = pathRef.Data.Value.HandlesIn.ToNativeArray();
                 var handlesOut = pathRef.Data.Value.HandlesOut.ToNativeArray();
                 var distances = pathRef.Data.Value.Distances.ToNativeArray();
                 var totalDistance = pathRef.Data.Value.TotalDistance;
                 
                 // Calculate position
                 var normalizedDistance = trainID * splitDistance;
                 var position = BezierHelpers.GetPosition(positions, handlesIn, handlesOut, distances, totalDistance,
                     normalizedDistance);
                 var normal = BezierHelpers.GetNormalAtPosition(positions, handlesIn, handlesOut, distances, totalDistance,
                     normalizedDistance);
                 
                 // Generate carriage
                 var carriage = ecb.Instantiate(carriagePrefab);
                 
                 // Position
                 var translation = new Translation {Value = position};
                 ecb.SetComponent(carriage, translation);
                 
                 // Rotation
                 var rotation = new Rotation {Value = quaternion.Euler(normal)};
                 ecb.SetComponent(carriage, rotation);
            }
        }).Run();
        
        ecb.Playback(EntityManager);

        Enabled = false;
    }
}
