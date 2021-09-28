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
        
        Entities.ForEach((Entity entity, in TrainSpawner trainSpawner) =>
        {
            ecb.DestroyEntity(entity);
            int trainCount = 1;
            //int trainLength = 1;

            for (int i = 0; i < trainCount; i++)
            {
                var trainInstance = ecb.Instantiate(trainSpawner.TrainPrefab);
                var trainMovement = new TrainMovement {position = 1 / (float)i};
                ecb.SetComponent(trainInstance, trainMovement);

                var carriageInstance = ecb.Instantiate(trainSpawner.CarriagePrefab);
                ecb.SetComponent(carriageInstance, new Parent{Value = trainInstance});
            }
            
        }).Run();
        
        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}
