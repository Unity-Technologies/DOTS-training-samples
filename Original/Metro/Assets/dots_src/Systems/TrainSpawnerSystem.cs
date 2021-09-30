using System.Collections;
using System.Collections.Generic;
using dots_src.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public partial class TrainSpawnerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        var lineStorageEntity = GetSingletonEntity<MarkersTag>();
        var lineEntities = GetBuffer<EntityBufferElement>(lineStorageEntity);

        Entities.ForEach((Entity entity, DynamicBuffer<TrainCountBufferElement> trainCounts, in TrainSpawner trainSpawner) =>
        {
            ecb.DestroyEntity(entity);
            const int trainLength = 5;

            for (var lineId = 0; lineId < lineEntities.Length; lineId++)
            {
                ref var splineDataBlobAsset = ref GetComponent<SplineDataReference>(lineEntities[lineId]).SplineBlobAsset.Value;

                int trainCount = trainCounts[lineId].Count;
                for (var i = 0; i < trainCount; i++)
                {
                    var trainInstance = ecb.Instantiate(trainSpawner.TrainPrefab);
                    var trainMovement = new TrainMovement
                    {
                        state = TrainMovemementStates.Starting,
                        position = (splineDataBlobAsset.equalDistantPoints.Length / trainCount) * i,
                        speed = 0f
                    };
                    ecb.SetComponent(trainInstance, trainMovement);
                    ecb.SetComponent(trainInstance, new LineEntity{Index = lineId});
                
                    for(int j = 0; j < trainLength; j++)
                    {
                        var carriageInstance = ecb.Instantiate(trainSpawner.CarriagePrefab);
                        ecb.SetComponent(carriageInstance, new TrainReference{Index = j, Train = trainInstance});
                    }

                }
            }
        }).Run();
        
        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}
