

using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

public partial class CreatureCollider : SystemBase
{
    private EntityCommandBufferSystem mECBSystem;

    protected override void OnCreate()
    {
        base.OnCreate();

        mECBSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }
    
    protected override void OnUpdate()
    {
        // Could make this only once, if cats never change
        var query = GetEntityQuery(typeof(Cat));

        if (query.IsEmpty)
        {
            return;
        }

        var ecb = mECBSystem.CreateCommandBuffer().AsParallelWriter();

        var cattiles = query.ToComponentDataArray<Tile>(Allocator.TempJob);

        Entities
            .WithAll<Mouse>()
            .ForEach((Entity entity, int entityInQueryIndex, in Tile mousetile) =>
            {
                foreach (var cattile in cattiles)
                {
                    // If cat and mouse on the same tile, destroy it - could change to a range based check
                    if (mousetile.Coords.x == cattile.Coords.x && mousetile.Coords.y == cattile.Coords.y)
                    {
                        ecb.DestroyEntity(entityInQueryIndex, entity);
                    }
                }
                
            }).ScheduleParallel();
        
        mECBSystem.AddJobHandleForProducer(Dependency);
    }
}
