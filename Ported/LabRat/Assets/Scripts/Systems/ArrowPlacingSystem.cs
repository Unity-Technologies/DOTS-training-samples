using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class ArrowPlacingSystem : SystemBase
{
    EntityCommandBufferSystem ECBSystem;

    protected override void OnCreate()
    {
        base.OnCreate();
        ECBSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var ecb = ECBSystem.CreateCommandBuffer().AsParallelWriter();
        Entities.ForEach((int entityInQueryIndex, in Entity placeArrowEventEntity, in PositionXZ position, in PlaceArrowEvent placeArrowEvent, in Direction direction) => {
            // Do the tile changing magic
            ecb.DestroyEntity(entityInQueryIndex, placeArrowEventEntity);
        }).ScheduleParallel();
        ECBSystem.AddJobHandleForProducer(Dependency);
    }
}
