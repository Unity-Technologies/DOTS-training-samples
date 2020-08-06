using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public class FarmerCollisionSystem : SystemBase
{
    private EntityQuery m_rocksQuery;
    private EntityCommandBufferSystem m_ECBSystem;

    protected override void OnCreate()
    {
        m_rocksQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new[]
           {
                ComponentType.ReadOnly<Rock>()                
            }
        });

        m_ECBSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {

        var rockPosition = m_rocksQuery.ToComponentDataArrayAsync<Rock>(Allocator.TempJob, out var rockPositionHandle);
        var rockEntities = m_rocksQuery.ToEntityArrayAsync(Allocator.TempJob, out var rocksEntitiesHandle);

        Dependency = JobHandle.CombineDependencies(Dependency, rockPositionHandle);
        Dependency = JobHandle.CombineDependencies(Dependency, rocksEntitiesHandle);

        var ecb = m_ECBSystem.CreateCommandBuffer().AsParallelWriter();
        var healthAccessor = GetComponentDataFromEntity<Health>();

        Entities
            .WithName("rock_farmer_collision_test")
            .WithDisposeOnCompletion(rockPosition)
            .WithDisposeOnCompletion(rockEntities)
            .WithNativeDisableParallelForRestriction(healthAccessor)
            .WithAll<Farmer>()
            .ForEach((int entityInQueryIndex, Entity farmerEntity, in Position2D farmerPosition) =>
            {
                
                var farmerRect = new Rect(farmerPosition.position.x - 0.5f, farmerPosition.position.y - 0.5f, 1f, 1f);
                for (int i = 0; i < rockPosition.Length; ++i)
                {
                    if (farmerRect.Overlaps(rockPosition[i].rectInt, true))
                    {
                        var currentHealth = healthAccessor[rockEntities[i]].Value;
                        if (currentHealth > 0)
                        {
                            healthAccessor[rockEntities[i]] = new Health { Value = currentHealth -= 20.0f };

                        }
                        else if(currentHealth <= 0.0f)
                        {
                            ecb.DestroyEntity(entityInQueryIndex, rockEntities[i]);
                        }
                        
                    }
                }

            }).ScheduleParallel();
        m_ECBSystem.AddJobHandleForProducer(Dependency);
    }
}