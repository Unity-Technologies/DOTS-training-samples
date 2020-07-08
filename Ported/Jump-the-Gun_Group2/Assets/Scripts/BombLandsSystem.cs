using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class BombLandsSystem : SystemBase
{

    private EntityCommandBufferSystem cbs;

    private EntityQuery needsSpawn;

    protected override void OnCreate() {
        cbs = World.GetExistingSystem<EndInitializationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var ecb = cbs.CreateCommandBuffer().ToConcurrent();

        Entities
           .WithNone<PlayerTag>()
           .ForEach((int entityInQueryIndex, Entity e, ref NormalisedMoveTime n, in MovementParabola p) => {

               //if the bomb has finished moving
               if(n.Value > 1) {
                   //destroy the bomb
                   ecb.DestroyEntity(entityInQueryIndex, e);
                   //reduce the height of the terrain at the position of the target
                   float3 target = p.Target;
                   //TODO reduce the height of the terrain
               }

           }).ScheduleParallel();

        cbs.AddJobHandleForProducer(Dependency);
    }
}