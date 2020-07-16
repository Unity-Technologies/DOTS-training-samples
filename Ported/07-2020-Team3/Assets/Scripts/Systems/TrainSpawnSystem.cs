using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class TrainSpawnSystem : SystemBase
{

    private EntityCommandBufferSystem _ecb;

    protected override void OnCreate()
    {
        _ecb = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();

    }
    protected override void OnUpdate()
    {

        if (!HasSingleton<TrainSpawner>())
            return;

        EntityCommandBuffer.Concurrent ecb = _ecb.CreateCommandBuffer().ToConcurrent();

        TrainSpawner trainSpawner = GetSingleton<TrainSpawner>();


        Entities.ForEach((int entityInQueryIndex, ref DynamicBuffer<TrackPoint> trackPoint, in Entity trackEntity) =>
        {
            Entity train = ecb.Instantiate(entityInQueryIndex, trainSpawner.trainPrefab);
            ecb.SetComponent(entityInQueryIndex, train, new TrainPosition
            {
                track = trackEntity
            });
        }).Schedule();

        //
        //  TODO - There's a better way for sure
        //
        Entities.ForEach((int entityInQueryIndex, in Entity entity, in TrainSpawner spawner) =>
        {
            ecb.DestroyEntity(entityInQueryIndex, entity);

        }).Schedule();

        _ecb.AddJobHandleForProducer(Dependency);
    }
}
