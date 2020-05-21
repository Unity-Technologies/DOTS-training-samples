using Unity.Entities;
using Unity.Entities.UniversalDelegates;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Profiling;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

[UpdateBefore(typeof(RockSystem))]
public class MoveSystem: SystemBase
{
    private BeginSimulationEntityCommandBufferSystem m_beginSimECBSystem;
    
    private static ProfilerMarker velocityMarker = new ProfilerMarker("velocityJobMarker");
    private static ProfilerMarker angularVelocityMarker = new ProfilerMarker("angularVelocityMarker");
    private static ProfilerMarker acellerationMarker = new ProfilerMarker("AccelMarker");
    private static ProfilerMarker rockBoundsMarker = new ProfilerMarker("rockBoundsMarker");
    private static ProfilerMarker canBoundsMarker = new ProfilerMarker("canBoundsMarker");
    protected override void OnCreate()
    {
        base.OnCreate();

        m_beginSimECBSystem = World.GetExistingSystem<BeginSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        

        float dt = Time.DeltaTime;

        var destroyEBC = m_beginSimECBSystem.CreateCommandBuffer().ToConcurrent();
        var respawnECB = m_beginSimECBSystem.CreateCommandBuffer().ToConcurrent();
        
        
        angularVelocityMarker.Begin();
        
        var angularVelJob = Entities
            .WithName("AngularVelocityJob")
            .ForEach((ref Rotation r, in AngularVelocity av) =>
            {
                var avQuat = quaternion.AxisAngle(av.UnitAxis,av.RadsPerSecond * dt);
                
                r.Value = math.mul(avQuat,r.Value);

            }).ScheduleParallel(Dependency);
        
        angularVelocityMarker.End();
        
        acellerationMarker.Begin();
        var acellJob = Entities
            .WithName("AccelerationJob")
            .ForEach((ref Velocity v, in Acceleration a) =>
            {
                v.Value += a.Value * dt;
                
            }).ScheduleParallel(Dependency);
        
        acellerationMarker.End();
        
        rockBoundsMarker.Begin();
        var rockBoundsJob = Entities
            .WithName("RockBoundsJob")
            .WithNone<CanTag>()
            .ForEach((Entity entity,
                int entityInQueryIndex,
                in Translation pos, in DestroyBoundsX boundsX, in DestroyBoundsY boundY) =>
            {
                //bounds X
                if(pos.Value.x < boundsX.Value.x ||
                   pos.Value.x > boundsX.Value.y)
                    destroyEBC.DestroyEntity(entityInQueryIndex,entity);
                
                //bounds Y
                if (pos.Value.y < boundY)
                {
                    if (HasComponent<RockReservedCan>(entity))
                    {
                        Entity can = GetComponent<RockReservedCan>(entity);
                        if (can != Entity.Null)
                        {
                            destroyEBC.RemoveComponent<CanReservedTag>(entityInQueryIndex,can);
                        }
                    }
                    
                    destroyEBC.DestroyEntity(entityInQueryIndex, entity);
                }
                
            }).ScheduleParallel(Dependency);
        
        rockBoundsMarker.End();
        
     
        m_beginSimECBSystem.AddJobHandleForProducer(rockBoundsJob);
   
        
        canBoundsMarker.Begin();

        var wrapAroundJob = Entities
            .WithName("CanWraparoundJob")
            .WithAll<Acceleration>()
            .ForEach((Entity entity,int entityInQueryIndex,
                ref Translation position, in DestroyBoundsX boundsX, in DestroyBoundsY boundsY, in CanInitSpeed canInitSpeed) =>
            {
                
                //check and wrap X bounds
                if (position.Value.x < boundsX.Value.x)
                {
                    position.Value.x = boundsX.Value.y - 1f;
                }
                else if(position.Value.x > boundsX.Value.y)
                {
                    position.Value.x = boundsX.Value.x + 1f;
                }
                
                //check and wrap Y bounds
                if (position.Value.y < boundsY.Value)
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
            }).ScheduleParallel(rockBoundsJob);
        
        canBoundsMarker.End();
        
        m_beginSimECBSystem.AddJobHandleForProducer(wrapAroundJob);

        velocityMarker.Begin();
        
        var velJob = Entities
            .WithName("VelocityJob")
            .WithNone<RockGrabbedTag>()
            .ForEach((ref Translation p, in Velocity v) =>
            {
                p.Value += v.Value * dt; 
                
            }).ScheduleParallel(JobHandle.CombineDependencies(wrapAroundJob,acellJob));
        
        velocityMarker.End();

        Dependency = JobHandle.CombineDependencies(velJob,angularVelJob);


    }
}
