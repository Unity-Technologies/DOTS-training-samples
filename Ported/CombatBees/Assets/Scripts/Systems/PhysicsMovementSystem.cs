using Unity.Entities;
using Unity.Collections;
using Unity.Transforms;
using Unity.Mathematics;

public partial class PhysicsMovementSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var system = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        var ecb = system.CreateCommandBuffer().AsParallelWriter();        

        var worldBoundsEntity = GetSingletonEntity<WorldBounds>();
        var bounds = GetComponent<WorldBounds>(worldBoundsEntity);
        var hiveBluePosition = bounds.AABB.Min +
                               new float3(bounds.HiveOffset / 2.0f, (bounds.AABB.Max.y - bounds.AABB.Min.y)/2.0f, (bounds.AABB.Max.z - bounds.AABB.Min.z)/2.0f);
        var hiveRedPosition = bounds.AABB.Max -
                              new float3(bounds.HiveOffset / 2.0f, (bounds.AABB.Max.y - bounds.AABB.Min.y)/2.0f, (bounds.AABB.Max.z - bounds.AABB.Min.z)/2.0f);
        
        float deltaTime = Time.DeltaTime;
        float multiplier = 7f;
        float floorY = 0.5f;
        
        Entities
            .WithAny<Food, Particle>()
            .WithNone<Grounded>()
            .ForEach((Entity entity, int entityInQueryIndex, ref Translation translation) =>
            {
                translation.Value.y -= deltaTime * multiplier;
                
                if (translation.Value.y < floorY)
                {
                    translation.Value.y = floorY;// this assume 0 is at the basis of the cylinder
                    ecb.AddComponent(entityInQueryIndex, entity, new Grounded{});
                }
            }).ScheduleParallel();
        
        system.AddJobHandleForProducer(Dependency);
    }
}