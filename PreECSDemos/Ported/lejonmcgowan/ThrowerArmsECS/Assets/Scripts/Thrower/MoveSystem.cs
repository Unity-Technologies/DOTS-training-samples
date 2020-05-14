using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

[UpdateBefore(typeof(RockSystem))]
public class MoveSystem: SystemBase
{
    private BeginSimulationEntityCommandBufferSystem m_beginSimECBSystem;
    
    protected override void OnCreate()
    {
        base.OnCreate();

        m_beginSimECBSystem = World.GetExistingSystem<BeginSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        float dt = Time.DeltaTime;

        var destroyXEBC = m_beginSimECBSystem.CreateCommandBuffer().ToConcurrent();
        var destroyYEBC = m_beginSimECBSystem.CreateCommandBuffer().ToConcurrent();
        var respawnECB = m_beginSimECBSystem.CreateCommandBuffer().ToConcurrent();
        
        Entities
            .WithName("VelocityJob")
            .WithNone<RockGrabbedTag>()
            .ForEach((ref Translation p, in Velocity v) =>
            {
                p.Value += v.Value * dt; 
                
            }).ScheduleParallel();
        
        Entities
            .WithName("AccelerationJob")
            .ForEach((ref Velocity v, in Acceleration a) =>
            {
                v.Value += a.Value * dt;
                
            }).ScheduleParallel();
        
        Entities
            .WithName("RockBoundsXJob")
            .WithNone<CanTag>()
            .ForEach((Entity entity,
                int entityInQueryIndex,
                ref Translation pos, in DestroyBoundsX boundsX) =>
            {
                if(pos.Value.x < boundsX.Value.x ||
                   pos.Value.x > boundsX.Value.y)
                    destroyXEBC.DestroyEntity(entityInQueryIndex,entity);
                
            }).ScheduleParallel();
        
        Entities
            .WithName("RockBoundsYJob")
            .WithNone<CanTag,RockGrabbedTag>()
            .ForEach((Entity entity,
                int entityInQueryIndex,
                ref Translation pos, in DestroyBoundsY boundY) =>
            {
                if (pos.Value.y < boundY)
                {
                    if (HasComponent<RockReservedCan>(entity))
                    {
                        Entity can = GetComponent<RockReservedCan>(entity);
                        if (can != Entity.Null)
                        {
                            destroyYEBC.RemoveComponent<CanReservedTag>(entityInQueryIndex,can);
                        }
                    }
                    
                    destroyYEBC.DestroyEntity(entityInQueryIndex, entity);
                }
            }).ScheduleParallel();
        
        Entities
            .WithName("CanWraparoundXJob")
            .WithAll<CanTag>()
            .WithNone<Acceleration>()
            .ForEach((Entity entity,int entityInQueryIndex,
                ref Translation position, in DestroyBoundsX bounds) =>
            {
                if (position.Value.x < bounds.Value.x)
                {
                    position.Value.x = bounds.Value.y - 1f;
                }
                else if(position.Value.x > bounds.Value.y)
                {
                    position.Value.x = bounds.Value.x + 1f;
                }
            }).ScheduleParallel();
        
        Entities
            .WithName("CanWraparoundYJob")
            .WithAll<Acceleration>()
            .ForEach((Entity entity,int entityInQueryIndex,
                ref Translation position, ref Velocity velocity, in DestroyBoundsY bounds, in CanInitSpeed canInitSpeed) =>
            {
                if (position.Value.y < bounds.Value)
                {
                    position.Value = canInitSpeed.initPos;
                    velocity.Value = canInitSpeed.initVel;
                    respawnECB.RemoveComponent<CanReservedTag>(entityInQueryIndex,entity);
                    respawnECB.RemoveComponent<Acceleration>(entityInQueryIndex,entity);
                }
            }).ScheduleParallel();
    }
}
