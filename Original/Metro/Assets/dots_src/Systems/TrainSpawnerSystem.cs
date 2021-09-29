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
            int trainCount = 3;
            int trainLength = 5;

            for (int l = 0; l < splineData.Value.splineBlobAssets.Length; l++)
            {
                for (int i = 0; i < trainCount; i++)
                {
                    var trainInstance = ecb.Instantiate(trainSpawner.TrainPrefab);
                    var trainMovement = new TrainMovement
                    {
                        state = TrainMovemementStates.Starting,
                        position = (splineData.Value.splineBlobAssets[l].points.Length / trainCount) * i
                    };
                    ecb.SetComponent(trainInstance, trainMovement);
                    ecb.SetComponent(trainInstance, new LineIndex{Index = l});
                
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
