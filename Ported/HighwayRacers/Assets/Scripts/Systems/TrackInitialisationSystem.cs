using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;
using System.Collections.Generic;

public class TrackInitialisationSystem : SystemBase
{
    BeginInitializationEntityCommandBufferSystem m_EntityCommandBufferSystem;
    EntityQuery m_TrackInfoQuery;

    protected override void OnCreate()
    {
        // Cache the BeginInitializationEntityCommandBufferSystem in a field, so we don't have to create it every frame
        m_EntityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
        m_TrackInfoQuery = GetEntityQuery(ComponentType.ReadOnly<TrackInfo>());
    }

    protected override void OnDestroy()
    {
        var trackInfos = new List<TrackInfo>();

        TrackInfo trackInfo = m_TrackInfoQuery.GetSingleton<TrackInfo>();

        trackInfo.Progresses.Dispose();
        trackInfo.Speeds.Dispose();
        trackInfo.Lanes.Dispose();
    }

    protected override void OnUpdate()
    {
        TrackProperties trackProperties = GetSingleton<TrackProperties>();

        var commandBuffer = m_EntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();

        Entities
            .WithoutBurst()
            .WithStructuralChanges()
            .ForEach((in TrackSpawner spawner) =>
            {
                var entity = EntityManager.CreateEntity();
                var trackInfo = new TrackInfo();
                // TODO: get right length
                int length = (int) trackProperties.TrackLength * trackProperties.NumberOfLanes;

                trackInfo.Progresses = new NativeArray<float>(length, Allocator.Persistent);
                trackInfo.Speeds = new NativeArray<float>(length, Allocator.Persistent);
                trackInfo.Lanes = new NativeArray<float>(length, Allocator.Persistent);
                EntityManager.AddComponentData(entity, trackInfo);
            }).Run();

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

                    // 3. scale to end position (start + length)
                    var scale = new float3(trackProperties.TrackLength, trackProperties.LaneWidth, 1);

                    commandBuffer.SetComponent(entityInQueryIndex, trackEntity, new LocalToWorld { Value = float4x4.TRS(position, quaternion.identity, scale) });

                    commandBuffer.RemoveComponent<Translation>(entityInQueryIndex, trackEntity);
                    commandBuffer.RemoveComponent<Rotation>(entityInQueryIndex, trackEntity);
                    commandBuffer.RemoveComponent<Scale>(entityInQueryIndex, trackEntity);
                    commandBuffer.RemoveComponent<NonUniformScale>(entityInQueryIndex, trackEntity);
                }

                // destroy the entity so we don't keep creating lanes
                commandBuffer.DestroyEntity(entityInQueryIndex, entity);

            }).ScheduleParallel();

        m_EntityCommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}
