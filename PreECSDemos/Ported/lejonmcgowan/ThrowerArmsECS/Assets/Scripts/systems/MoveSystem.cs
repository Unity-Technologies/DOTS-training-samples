using Unity.Entities;
using Unity.Entities.UniversalDelegates;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Profiling;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

[UpdateBefore(typeof(RockSystem))]
public class MoveSystem : SystemBase
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

        var destroyEBC = m_beginSimECBSystem.CreateCommandBuffer().ToConcurrent();
        var respawnECB = m_beginSimECBSystem.CreateCommandBuffer().ToConcurrent();
        
        var rockBoundsJob = Entities
            .WithName("RockBoundsJob")
            .WithNone<CanTag>()
            .ForEach((Entity entity,
                int entityInQueryIndex,
                in Translation pos, in DestroyBoundsX boundsX, in DestroyBoundsY boundY) =>
            {
                //bounds X
                if (pos.Value.x < boundsX.Value.x ||
                    pos.Value.x > boundsX.Value.y)
                    destroyEBC.DestroyEntity(entityInQueryIndex, entity);

                //bounds Y
                if (pos.Value.y < boundY)
                {
                    if (HasComponent<RockReservedCan>(entity))
                    {
                        Entity can = GetComponent<RockReservedCan>(entity);
                        if (can != Entity.Null)
                        {
                            destroyEBC.RemoveComponent<CanReservedTag>(entityInQueryIndex, can);
                        }
                    }

                    destroyEBC.DestroyEntity(entityInQueryIndex, entity);
                }
            }).ScheduleParallel(Dependency);


        m_beginSimECBSystem.AddJobHandleForProducer(rockBoundsJob);


        var wrapAroundJob = Entities
            .WithName("CanWraparoundJob")
            .WithAll<Acceleration>()
            .ForEach((Entity entity, int entityInQueryIndex,
                ref Translation position, in DestroyBoundsX boundsX, in DestroyBoundsY boundsY,
                in CanInit canInitParams) =>
            {
                //check and wrap X bounds
                if (position.Value.x < boundsX.Value.x)
                {
                    position.Value.x = boundsX.Value.y - 1f;
                }
                else if (position.Value.x > boundsX.Value.y)
                {
                    position.Value.x = boundsX.Value.x + 1f;
                }

                //check and wrap Y bounds
                if (position.Value.y < boundsY.Value)
                {
                    respawnECB.SetComponent(entityInQueryIndex, entity, new Translation
                    {
                        Value = canInitParams.initPos
                    });
                    respawnECB.SetComponent(entityInQueryIndex, entity, new Velocity
                    {
                        Value = canInitParams.initVel
                    });
                    respawnECB.SetComponent(entityInQueryIndex, entity, new Rotation
                    {
                        Value = quaternion.identity
                    });

                    respawnECB.RemoveComponent<CanReservedTag>(entityInQueryIndex, entity);
                    respawnECB.SetComponent(entityInQueryIndex, entity,new Acceleration());
                    respawnECB.SetComponent(entityInQueryIndex, entity, new AngularVelocity());
                }
            }).ScheduleParallel(rockBoundsJob);


        m_beginSimECBSystem.AddJobHandleForProducer(wrapAroundJob);
        
        var physicsJob = Entities
            .WithName("PreVelocityPhysicsJob")
            .ForEach((Entity entity,
                ref Translation p,
                ref Rotation r,
                ref Velocity v,
                in AngularVelocity av,
                in Acceleration a) =>
            {
                var avQuat = quaternion.AxisAngle(av.UnitAxis, av.RadsPerSecond * dt);

                r.Value = math.mul(avQuat, r.Value);
                v.Value += a.Value * dt;

                if (!HasComponent<RockGrabbedTag>(entity))
                {
                    p.Value += v.Value * dt;
                }
            }).ScheduleParallel(wrapAroundJob);
        
        Dependency = physicsJob;

    }
}