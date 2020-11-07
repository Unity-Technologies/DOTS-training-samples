using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateAfter(typeof(MazeGenerator))]
public class CapsulePickup : SystemBase
{
    private EndSimulationEntityCommandBufferSystem _endSimulationEntityCommandBufferSystem;

    protected override void OnCreate()
    {
        base.OnCreate();
        _endSimulationEntityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var player = GetSingletonEntity<PlayerTag>();
        var playerPos = EntityManager.GetComponentData<Position>(player);

        var ecb = _endSimulationEntityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter();

        Entities
            .WithAll<CapsuleRotation>()
            .ForEach((Entity entity, int entityInQueryIndex, in Position pos) =>
        {
            float dist = math.distance(playerPos.Value, pos.Value);

            if (dist <= 0.2f)
            {
                ecb.DestroyEntity(entityInQueryIndex, entity);
            }
        }).ScheduleParallel();


        Entities
            .WithAll<ZombieTag>()
            .ForEach((Entity entity, int entityInQueryIndex, in Position pos) =>
            {
                float dist = math.distance(playerPos.Value, pos.Value);

                if (dist <= 0.2f)
                {
                   // ecb.DestroyEntity(entityInQueryIndex, entity);
                   ecb.SetComponent(entityInQueryIndex, player, new Position());
                   ecb.SetComponent(entityInQueryIndex, player, new Direction());
                }
            }).ScheduleParallel();

        _endSimulationEntityCommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}
