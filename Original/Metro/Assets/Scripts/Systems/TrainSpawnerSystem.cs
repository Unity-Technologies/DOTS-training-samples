using System.Collections;
using System.Collections.Generic;
using dots_src.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

public partial class TrainSpawnerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        
        var splineData = GetSingleton<SplineDataReference>().BlobAssetReference;
        var settings = GetSingleton<Settings>().SettingsBlobRef;

        
        Entities.ForEach((Entity entity, in TrainSpawner trainSpawner) =>
        {
            ecb.DestroyEntity(entity);
            ref var lineColors = ref settings.Value.LineColors ;

            for (var lineId = 0; lineId < splineData.Value.splineBlobAssets.Length; lineId++)
            {
                ref var splineDataBlobAsset = ref splineData.Value.splineBlobAssets[lineId];
                var trainCount = splineDataBlobAsset.TrainCount;
                
                var previousTrain = Entity.Null;
                var firstTrain = Entity.Null;
                
                var lineColor = lineColors[lineId % lineColors.Length];

                for (var i = 0; i < trainCount; i++)
                {
                    var trainInstance = ecb.CreateEntity();
                    ecb.SetName(trainInstance, $"Train {lineId}-{i}");
                    ecb.AddComponent(trainInstance, new TrainState{State = TrainMovementStates.Starting});
                    ecb.AddComponent(trainInstance, new TrainMovement {speed = 0f});
                    ecb.AddComponent(trainInstance, new TrainPosition {Value = (splineDataBlobAsset.equalDistantPoints.Length / trainCount) * i,});
                    ecb.AddComponent(trainInstance, new LineIndex{Index = lineId});
                    ecb.AddComponent<PlatformRef>(trainInstance);
                    if (previousTrain == Entity.Null)
                        firstTrain = trainInstance;
                    else
                        ecb.AddComponent(previousTrain, new TrainInFront {Train = trainInstance});

                    for(var j = 0; j < splineDataBlobAsset.CarriagesPerTrain; j++)
                    {
                        var carriageInstance = ecb.Instantiate(trainSpawner.CarriagePrefab);
                        ecb.SetName(carriageInstance, $"Carriage {lineId}-{i}-{j}");
                        ecb.AddComponent(carriageInstance, new URPMaterialPropertyBaseColor() {Value = lineColor});
                        ecb.SetComponent(carriageInstance, new TrainReference { Train = trainInstance });
                        ecb.SetComponent(carriageInstance, new CarriageIndex { Value = j });
                        ecb.SetName(carriageInstance, $"Car {lineId}-{i}-{j}");
                    }

                    previousTrain = trainInstance;
                }

                // let last train (previousTrain) point to first train
                if (trainCount != 0)
                    ecb.AddComponent(previousTrain, new TrainInFront{Train = firstTrain});
            }
        }).Run();
        
        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}
