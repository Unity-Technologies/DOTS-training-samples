using System.Linq;
using MetroECS.Trains;
using MetroECS.Trains.States;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using Random = Unity.Mathematics.Random;

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

            for (var trainID = 0; trainID < nativePathData.NumberOfTrains; trainID++)
            {
                var carriageCount = random.NextInt(nativePathData.MaxCarriages / 2, nativePathData.MaxCarriages);
                var normalizedTrainPosition = trainID * splitDistance;

                // Create train
                var trainEntity = ecb.CreateEntity();
                var trainData = new Train {ID = trainID, Position = normalizedTrainPosition, CarriageCount = carriageCount, Path = pathDataEntity};
                ecb.AddComponent(trainEntity, trainData);
                
                var inMotionTag = new TrainInMotionTag();
                ecb.AddComponent(trainEntity, inMotionTag);
                
                for (var carriageID = 0; carriageID < carriageCount; carriageID++)
                {
                    // Generate carriage
                    var carriageEntity = ecb.Instantiate(carriagePrefab);
                    var carriageData = new Carriage {ID = carriageID, Train = trainEntity};
                    ecb.SetComponent(carriageEntity, carriageData);
                }
            }
        }).Run();
        
        ecb.Playback(EntityManager);

        Enabled = false;
    }
}
