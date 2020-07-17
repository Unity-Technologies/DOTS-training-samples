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
            for (int i = 0; i < trainSpawner.numberOfTrainsPerTrack; i++)
            {
                Entity train = ecb.Instantiate(entityInQueryIndex, trainSpawner.trainPrefab);

                float position = i * trackPoint.Length / trainSpawner.numberOfTrainsPerTrack;

                ecb.SetComponent(entityInQueryIndex, train, new TrainPosition
                {
                    track = trackEntity,
                    position = position
                });
                /*
                ecb.SetComponent(entityInQueryIndex, train, new TrainState
                {
                    inbound = i > trackPoint.Length / 2
                });
                */
            }
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
