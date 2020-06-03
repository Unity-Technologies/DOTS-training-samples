using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class TrackInitialisationSystem : SystemBase
{
    BeginInitializationEntityCommandBufferSystem m_EntityCommandBufferSystem;

    protected override void OnCreate()
    {
        // Cache the BeginInitializationEntityCommandBufferSystem in a field, so we don't have to create it every frame
        m_EntityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        TrackProperties trackProperties = GetSingleton<TrackProperties>();

        var commandBuffer = m_EntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();

        Entities
            .WithoutBurst()
            .ForEach((Entity entity, int entityInQueryIndex, in TrackSpawner spawner) =>
            {
                for (int i = 0; i < trackProperties.NumberOfLanes; ++i)
                {
                    // 1. generate tracklane prefab
                    Entity trackEntity = commandBuffer.Instantiate(entityInQueryIndex, spawner.TrackPrefab);

                    // 2. set correct start position
                    var position = new float3(
                        trackProperties.TrackStartingPoint.x,
                        trackProperties.TrackStartingPoint.y + (trackProperties.LaneWidth + trackProperties.SeparationWidth) * i,
                        40);

                    commandBuffer.SetComponent(entityInQueryIndex, trackEntity, new Translation { Value = position });

                    // 3. scale to end position (start + length)
                    var scale = new float3(trackProperties.TrackLength, trackProperties.LaneWidth, 1);
                    commandBuffer.AddComponent(entityInQueryIndex, trackEntity, new NonUniformScale { Value = scale });
                }

                // destroy the entity so we don't keep creating lanes
                commandBuffer.DestroyEntity(entityInQueryIndex, entity);

            }).ScheduleParallel();

        m_EntityCommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}
