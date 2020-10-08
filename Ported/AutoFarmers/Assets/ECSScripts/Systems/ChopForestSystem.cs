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
                    float3 forestModelScale = GetComponent<NonUniformScale>(GetComponent<ForestDisplay>(forestEntity).Value).Value;
                    ecb.AddComponent(entityInQueryIndex, entity, new ChoppingTask(){ Target = forestEntity, Completion = 0, OriginalScale = forestModelScale});
                }
            }).ScheduleParallel();

        float choppingDelay = 2f;
        float deltaTime = Time.DeltaTime;
        Entities
            .WithName("chopping_system_farmers")
            .WithAll<Farmer>()
            .ForEach((
                Entity entity, 
                int entityInQueryIndex,
                ref ChoppingTask task, 
                in Position position) =>
            {
                Entity plainTarget = task.Target;
                Entity forestModel = GetComponent<ForestDisplay>(plainTarget).Value;
                Forest forest = GetComponent<Forest>(plainTarget);

                task.Completion = task.Completion + deltaTime / choppingDelay;
                if(task.Completion > forest.Health)
                {
                    ecb.DestroyEntity(entityInQueryIndex, forestModel);
                    ecb.RemoveComponent<Forest>(entityInQueryIndex, plainTarget);
                    ecb.RemoveComponent<ForestDisplay>(entityInQueryIndex, plainTarget);
                    ecb.RemoveComponent<ChoppingTask>(entityInQueryIndex, entity);
                }
                else
                {
                    float3 scale = (1f - task.Completion) * task.OriginalScale;
                    scale.x = math.floor(25f * scale.x) / 25f;
                    scale.y = math.floor(25f * scale.y) / 25f;
                    scale.z = math.floor(25f * scale.z) / 25f;
                    ecb.SetComponent(entityInQueryIndex,forestModel, new NonUniformScale{Value = scale});
                }
            }).ScheduleParallel();
        
        m_ECBSystem.AddJobHandleForProducer(Dependency);
        
    }
}
