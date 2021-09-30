using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public partial class TrainSpawnerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        
        var splineData = GetSingleton<SplineDataReference>().BlobAssetReference;
        
        Entities.ForEach((Entity entity, DynamicBuffer<TrainCountBufferElement> trainCounts, in TrainSpawner trainSpawner) =>
        {
            ecb.DestroyEntity(entity);
            const int carriagesPerTrain = 5;

            for (var lineId = 0; lineId < splineData.Value.splineBlobAssets.Length; lineId++)
            {
                ref var splineDataBlobAsset = ref splineData.Value.splineBlobAssets[lineId];
                var trainCount = trainCounts[lineId].Count;
                
                var previousTrain = Entity.Null;
                var firstTrain = Entity.Null;
                
                for (var i = 0; i < trainCount; i++)
                {
                    var trainInstance = ecb.Instantiate(trainSpawner.TrainPrefab);
                    ecb.AddComponent(trainInstance, new TrainState{State = TrainMovementStates.Starting});
                    ecb.SetComponent(trainInstance, new TrainMovement {speed = 0f});
                    ecb.SetComponent(trainInstance, new TrainPosition {position = (splineDataBlobAsset.equalDistantPoints.Length / trainCount) * i,});
                    ecb.SetComponent(trainInstance, new LineIndex{Index = lineId});

                    if (previousTrain != Entity.Null)
                        ecb.SetComponent(previousTrain, new TrainInFront { Train = trainInstance });
                    else
                        firstTrain = trainInstance;

                    for(var j = 0; j < carriagesPerTrain; j++)
                    {
                        var carriageInstance = ecb.Instantiate(trainSpawner.CarriagePrefab);
                        ecb.SetComponent(carriageInstance, new TrainReference { Train = trainInstance });
                        ecb.SetComponent(carriageInstance, new CarriageIndex { Value = j });
                    }

                    previousTrain = trainInstance;
                }

                // let last train (previousTrain) point to first train
                if (trainCount != 0)
                    ecb.SetComponent(previousTrain, new TrainInFront{Train = firstTrain});
            }
        }).Run();
        
        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}
