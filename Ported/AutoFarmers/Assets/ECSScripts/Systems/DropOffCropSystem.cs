using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class DropOffCropSystem : SystemBase 
{
    EntityCommandBufferSystem m_ECBSystem;
    
    protected override void OnCreate()
    {
        m_ECBSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();    
    }

    protected override void OnUpdate()
    {
        var ecb = m_ECBSystem.CreateCommandBuffer().AsParallelWriter();
        const float reachDistance = 0.2f; 
        
        Entities
            .WithName("dropoff_system_farmerscheck")
            .WithAll<Farmer>()
            .ForEach((
                Entity entity, 
                int entityInQueryIndex,
                ref DropOffCropTask task, 
                ref TargetEntity target, 
                in Position position) =>
            {
                float distanceToTarget = math.distance(position.Value, target.targetPosition);
                if(distanceToTarget < reachDistance)
                {
                    ecb.RemoveComponent<DropOffCropTask>(entityInQueryIndex, entity);
                    ecb.RemoveComponent<TargetEntity>(entityInQueryIndex, entity);
                }
            }).ScheduleParallel();
        
        Entities
            .WithName("dropoff_system_dropcrop")
            .WithAll<Crop>()
            .ForEach((
                Entity entity, 
                int entityInQueryIndex,
                ref CropCarried cropCarried, 
                ref TargetEntity target, 
                in Translation translation) =>
            {
                float2 translation2d = new float2(translation.Value.x, translation.Value.z);
                float distanceToTarget = math.distance(translation2d, target.targetPosition);
                if(distanceToTarget < reachDistance)
                {
                    ecb.AddComponent<CropDropOff>(entityInQueryIndex, entity, 
                        new CropDropOff { 
                                    Completion = 0f,
                                    FromPosition = translation.Value, 
                                    ToPosition = new float3(target.targetPosition.x, 0, target.targetPosition.y)});
                    
                    ecb.RemoveComponent<CropCarried>(entityInQueryIndex, entity);
                    ecb.RemoveComponent<TargetEntity>(entityInQueryIndex, entity);
                }
            }).ScheduleParallel();

        float dropDelay = 3f;
        float deltaTime = Time.DeltaTime;
        Entities
            .WithName("dropoff_system_removecrop")
            .WithAll<Crop>()
            .ForEach((
                Entity entity, 
                int entityInQueryIndex,
                ref CropDropOff dropOff,
                ref NonUniformScale scale,
                ref Translation translation) =>
            {
                dropOff.Completion += deltaTime / dropDelay;

                if(dropOff.Completion > 1f)
                {
                    ecb.DestroyEntity(entityInQueryIndex, entity);
                }
                else
                {
                    scale.Value = (1f - dropOff.Completion) * scale.Value;
                    translation.Value = dropOff.FromPosition + dropOff.Completion * (dropOff.ToPosition - dropOff.FromPosition);
                }

            }).ScheduleParallel(); 
        
        m_ECBSystem.AddJobHandleForProducer(Dependency);
    }
}
