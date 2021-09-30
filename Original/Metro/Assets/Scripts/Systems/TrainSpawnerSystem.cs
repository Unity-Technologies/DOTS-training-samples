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
        
        Entities.ForEach((Entity entity, in TrainSpawner trainSpawner) =>
        {
            ecb.DestroyEntity(entity);
            const int carriagesPerTrain = 5;

            for (var lineId = 0; lineId < splineData.Value.splineBlobAssets.Length; lineId++)
            {
                ref var splineDataBlobAsset = ref splineData.Value.splineBlobAssets[lineId];
                var trainCount = splineDataBlobAsset.TrainCount;
                
                var previousTrain = Entity.Null;
                var firstTrain = Entity.Null;
                
                for (var i = 0; i < trainCount; i++)
                {
                    var trainInstance = ecb.CreateEntity();
                    ecb.AddComponent(trainInstance, new TrainInFront());
                    ecb.AddComponent(trainInstance, new TrainState{State = TrainMovementStates.Starting});
                    ecb.AddComponent(trainInstance, new TrainMovement {speed = 0f});
                    ecb.AddComponent(trainInstance, new TrainPosition {Value = (splineDataBlobAsset.equalDistantPoints.Length / trainCount) * i,});
                    ecb.AddComponent(trainInstance, new LineIndex{Index = lineId});

                    if (previousTrain == Entity.Null)
                        firstTrain = trainInstance;
                    else
                        ecb.SetComponent(previousTrain, new TrainInFront {Train = trainInstance});

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
