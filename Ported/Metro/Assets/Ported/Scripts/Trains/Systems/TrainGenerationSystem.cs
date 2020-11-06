using MetroECS.Trains.States;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Random = Unity.Mathematics.Random;

namespace MetroECS.Trains
{
    public class TrainGenerationSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var carriagePrefab = GetSingleton<MetroData>().CarriagePrefab;
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            Entities.ForEach((in Entity pathDataEntity, in PathDataRef pathRef) =>
            {
                var nativePathData = pathRef.ToNativePathData();

                var splitDistance = 1f / nativePathData.NumberOfTrains;

                for (var trainID = 0; trainID < nativePathData.NumberOfTrains; trainID++)
                {
                    var normalizedTrainPosition = trainID * splitDistance;
                    var regionIndex = BezierHelpers.GetRegionIndex(nativePathData.Positions, nativePathData.Distances,
                        normalizedTrainPosition * nativePathData.TotalDistance);
                    regionIndex = (regionIndex + 1) % nativePathData.MarkerTypes.Length;

                    // Create train
                    var trainEntity = ecb.CreateEntity();
                    var trainData = new Train {ID = trainID, Position = normalizedTrainPosition, TargetIndex = regionIndex, CarriageCount = nativePathData.CarriagesPerTrain, Path = pathDataEntity};
                    ecb.AddComponent(trainEntity, trainData);
                
                    var inMotionTag = new TrainInMotionTag();
                    ecb.AddComponent(trainEntity, inMotionTag);
                
                    for (var carriageID = 0; carriageID < trainData.CarriageCount; carriageID++)
                    {
                        // Generate carriage
                        var carriageEntity = ecb.Instantiate(carriagePrefab);
                        var carriageData = new Carriage {ID = carriageID, Train = trainEntity, Position = normalizedTrainPosition};
                        ecb.SetComponent(carriageEntity, carriageData);
                    }
                }
            }).Run();
        
            ecb.Playback(EntityManager);

            Enabled = false;
        }
    }
}