using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class ChopForestSystem : SystemBase
{
    EntityCommandBufferSystem m_ECBSystem;
    
    protected override void OnCreate()
    {
        m_ECBSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }
    
    protected override void OnUpdate()
    {
        var ecb = m_ECBSystem.CreateCommandBuffer().AsParallelWriter();
        const float reachDistance = 0.5f;
        
        Entities
            .WithName("chopforest_system_farmers")
            .WithAll<Farmer>()
            .ForEach((
                Entity entity, 
                int entityInQueryIndex,
                ref ChopForestTask task, 
                ref TargetEntity target, 
                in Position position) =>
            {
                float distanceToTarget = math.distance(position.Value, target.targetPosition);
                if(distanceToTarget < reachDistance)
                {
                    Entity forestEntity = target.target;
                    
                    //Updating farmer task
                    ecb.RemoveComponent<ChopForestTask>(entityInQueryIndex, entity);
                    ecb.RemoveComponent<TargetEntity>(entityInQueryIndex, entity);
                    //Add Chopping Tag
                    Entity forestDisplayEntity = GetComponent<ForestDisplay>(forestEntity).Value;
                    float3 forestModelScale = GetComponent<NonUniformScale>(forestDisplayEntity).Value;
                    ecb.AddComponent(entityInQueryIndex, forestDisplayEntity, new DestroyForest{Completion = 0, OriginalScale = forestModelScale, DisplayParentEntity = forestEntity});
                    //ecb.AddComponent(entityInQueryIndex, entity, new ChoppingTask(){ Target = forestEntity, Completion = 0, OriginalScale = forestModelScale});
                    ecb.AddComponent(entityInQueryIndex, entity, new ChoppingTask(){ Target = forestEntity });
                }
            }).ScheduleParallel();
        
        Entities.
            ForEach((Entity entity, 
                int entityInQueryIndex,
                ref DestroyForest destroyForest, 
                ref NonUniformScale forestScale) =>
            {
                if(destroyForest.Completion > 1f)
                {
                    ecb.SetComponent(entityInQueryIndex, destroyForest.DisplayParentEntity, new ForestDisplay{Value = Entity.Null});
                    ecb.DestroyEntity(entityInQueryIndex, entity);
                }
            }).ScheduleParallel();
        
        Entities
            .WithName("chopping_system_farmers")
            .WithAll<Farmer>()
            .ForEach((
                Entity entity, 
                int entityInQueryIndex,
                ref ChoppingTask task) =>
            {
                Entity plainTarget = task.Target;
                Entity forestModel = GetComponent<ForestDisplay>(plainTarget).Value;
                if(forestModel.Equals(Entity.Null))
                {
                    Forest forest = GetComponent<Forest>(plainTarget);
                    
                    ecb.RemoveComponent<Forest>(entityInQueryIndex, plainTarget);
                    ecb.RemoveComponent<ForestDisplay>(entityInQueryIndex, plainTarget);
                    ecb.RemoveComponent<ChoppingTask>(entityInQueryIndex, entity);
                }
                
            }).ScheduleParallel();
        
        m_ECBSystem.AddJobHandleForProducer(Dependency);
        
    }
}
