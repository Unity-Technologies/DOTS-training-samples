using Unity.Entities;
using Unity.Entities.UniversalDelegates;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

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
            .WithName("AngularVelocityJob")
            .ForEach((ref Rotation r, in AngularVelocity av) =>
            {
                var avQuat = quaternion.AxisAngle(av.UnitAxis,av.RadsPerSecond * dt);
                
                r.Value = math.mul(avQuat,r.Value);

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
        
        m_beginSimECBSystem.AddJobHandleForProducer(Dependency);
        
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
                in Translation position, in DestroyBoundsY bounds, in CanInitSpeed canInitSpeed) =>
            {
                if (position.Value.y < bounds.Value)
                {
                    respawnECB.SetComponent(entityInQueryIndex, entity, new Translation
                    {
                        Value = canInitSpeed.initPos
                    });
                    respawnECB.SetComponent(entityInQueryIndex, entity, new Velocity
                    {
                        Value = canInitSpeed.initVel
                    });
                    respawnECB.SetComponent(entityInQueryIndex, entity, new Rotation
                    {
                        Value = quaternion.identity
                    });
                    
                    respawnECB.RemoveComponent<CanReservedTag>(entityInQueryIndex,entity);
                    respawnECB.RemoveComponent<Acceleration>(entityInQueryIndex,entity);
                    respawnECB.RemoveComponent<AngularVelocity>(entityInQueryIndex,entity);
                }
            }).ScheduleParallel();
    }
}
