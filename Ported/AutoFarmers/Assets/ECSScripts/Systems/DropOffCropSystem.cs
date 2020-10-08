using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

//[UpdateAfter(typeof(FarmerMoveSystem))]
public class DropOffCropSystem : SystemBase 
{
    EntityQuery m_AvailableDepotsQuery;
    EntityCommandBufferSystem m_ECBSystem;
    
    protected override void OnCreate()
    {
        m_ECBSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();    
        m_AvailableDepotsQuery = GetEntityQuery(typeof(Depot));
    }

    protected override void OnUpdate()
    {
        var gameStateEntity = GetSingletonEntity<GameState>();
        var score = GetComponent<Score>(gameStateEntity);
        var gameTime = GetSingleton<GameTime>();
        var ecb = m_ECBSystem.CreateCommandBuffer().AsParallelWriter();
        const float reachDistance = 0.5f; 
        
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
                    
                    ecb.AddComponent<DestroyCrop>(entityInQueryIndex, entity);
                    
                    ecb.RemoveComponent<CropCarried>(entityInQueryIndex, entity);
                    ecb.RemoveComponent<Assigned>(entityInQueryIndex, target.target);
                    ecb.RemoveComponent<TargetEntity>(entityInQueryIndex, entity);
                }
            }).ScheduleParallel();
        
        float dropDelay = 2f;
        float deltaTime = gameTime.DeltaTime;
        
        int depotCount = m_AvailableDepotsQuery.CalculateEntityCount();
        NativeArray<Entity> depotsEntities = m_AvailableDepotsQuery.ToEntityArray(Allocator.TempJob);
        
        Entities
            .WithName("dropoff_system_removecrop")
            .WithStructuralChanges()
            .WithAll<Crop>()
            .WithAll<DestroyCrop>()
            .ForEach((
                Entity entity, 
                ref CropDropOff dropOff,
                ref NonUniformScale scale,
                ref Translation translation) =>
            {
                dropOff.Completion += deltaTime / dropDelay;
        
                if(dropOff.Completion > 1f)
                {
                    EntityManager.DestroyEntity(entity);
                
                    float random;
                    {
                        float2 p = translation.Value.xz;
                        float3 p3 = math.frac(p.xyx * 0.1031f);
                        p3 += math.dot(p3, p3.yzx + 33.33f);
                        random = math.frac((p3.x + p3.y) * p3.z);
                    }
                    random *= depotCount;
                    int randomInt = (int) random;
                    
                    int updatedScore = score.Value + 1;
                    EntityManager.SetComponentData(gameStateEntity, new Score {Value = updatedScore});
                    if (updatedScore % 5 == 0)
                    {
                        EntityManager.AddComponent<DepotCanSpawn>(depotsEntities[randomInt]);
                    }
                }
                else
                {
                    scale.Value = (1f - dropOff.Completion) * scale.Value;
                    translation.Value = dropOff.FromPosition + dropOff.Completion * (dropOff.ToPosition - dropOff.FromPosition);
                }
        
            }).Run(); 
        
        depotsEntities.Dispose(Dependency);
        
        m_ECBSystem.AddJobHandleForProducer(Dependency);
    }
}
