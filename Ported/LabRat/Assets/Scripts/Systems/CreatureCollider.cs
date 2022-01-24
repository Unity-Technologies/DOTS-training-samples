

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
        EntityQuery query = GetEntityQuery(typeof(Cat));
        var cats = query.ToEntityArray(Allocator.TempJob);

        if (query.IsEmpty)
        {
            return;
        }
        
        var ecb = mECBSystem.CreateCommandBuffer().AsParallelWriter();

        Entities
            .WithAll<Mouse>()
            .ForEach((Entity entity, int entityInQueryIndex, in Tile mousetile) =>
            {
                foreach (var cat in cats)
                {
                    Tile cattile = GetComponent<Tile>(cat);
                    // If cat and mouse on the same tile, destroy it - could change to a range based check
                    if (mousetile.Value.x == cattile.Value.x && mousetile.Value.y == cattile.Value.y)
                    {
                        ecb.DestroyEntity(entityInQueryIndex, entity);
                    }
                }
                
            }).ScheduleParallel();
        
        mECBSystem.AddJobHandleForProducer(Dependency);
    }
}
