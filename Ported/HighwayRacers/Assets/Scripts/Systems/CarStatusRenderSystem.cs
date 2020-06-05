using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Rendering;

[UpdateInGroup(typeof(PresentationSystemGroup))]
public class CarStatusRenderSystem : SystemBase
{
    bool m_InitDone = false;
    RenderMesh m_InitRenderMesh;
    RenderMesh m_BlockedRenderMesh;
    RenderMesh m_DefaultSpeedRenderMesh;
    RenderMesh m_AcceleratingRenderMesh;

    EntityCommandBufferSystem m_EntityCommandBufferSystem;

    protected override void OnCreate()
    {
        m_EntityCommandBufferSystem = World.GetExistingSystem<BeginInitializationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        if (!m_InitDone)
        {
            m_InitDone = true;

            CarConfigurations carConfig = GetSingleton<CarConfigurations>();
            var carPrefab = carConfig.CarPrefab;

            m_InitRenderMesh = EntityManager.GetSharedComponentData<RenderMesh>(carPrefab);

            m_BlockedRenderMesh = m_InitRenderMesh;
            m_BlockedRenderMesh.material = CarStatusDisplayManager.Instance.BlockedStatusMaterial;

            m_DefaultSpeedRenderMesh = m_InitRenderMesh;
            m_DefaultSpeedRenderMesh.material = CarStatusDisplayManager.Instance.DefaultSpeedStatusMaterial;

            m_AcceleratingRenderMesh = m_InitRenderMesh;
            m_AcceleratingRenderMesh.material = CarStatusDisplayManager.Instance.AccelerationStatusMaterial;
        }
        
        var query = GetEntityQuery(ComponentType.ReadOnly<BlockedState>(), ComponentType.ReadWrite<RenderMesh>());
        query.SetSharedComponentFilter(m_InitRenderMesh);
        EntityManager.SetSharedComponentData(query, m_BlockedRenderMesh);

        query = GetEntityQuery(ComponentType.ReadOnly<BlockedState>(), ComponentType.ReadWrite<RenderMesh>());
        query.SetSharedComponentFilter(m_DefaultSpeedRenderMesh);
        EntityManager.SetSharedComponentData(query, m_BlockedRenderMesh);

        query = GetEntityQuery(ComponentType.ReadOnly<BlockedState>(), ComponentType.ReadWrite<RenderMesh>());
        query.SetSharedComponentFilter(m_AcceleratingRenderMesh);
        EntityManager.SetSharedComponentData(query, m_BlockedRenderMesh);

        var ecb = m_EntityCommandBufferSystem.CreateCommandBuffer();

        var defaultSpeedRenderMesh = m_DefaultSpeedRenderMesh;
        var acceleratingRenderMesh = m_AcceleratingRenderMesh;

        Entities
            .WithSharedComponentFilter(defaultSpeedRenderMesh)
            .WithoutBurst()
            .WithNone<BlockedState>()
            .ForEach((Entity carEntity, in CarProperties carProperty, in Speed speed) =>
            {
                if (carProperty.DefaultSpeed > speed.Value)
                {
                    ecb.SetSharedComponent(carEntity, acceleratingRenderMesh);
                }
            }).Run();

        Entities
            .WithSharedComponentFilter(acceleratingRenderMesh)
            .WithoutBurst()
            .WithNone<BlockedState>()
            .ForEach((Entity carEntity, in CarProperties carProperty, in Speed speed) =>
            {
                if (carProperty.DefaultSpeed <= speed.Value)
                {
                    ecb.SetSharedComponent(carEntity, defaultSpeedRenderMesh);
                }
            }).Run();

        Entities
            .WithSharedComponentFilter(m_BlockedRenderMesh)
            .WithoutBurst()
            .WithNone<BlockedState>()
            .ForEach((Entity carEntity, in CarProperties carProperty, in Speed speed) =>
            {
                if (carProperty.DefaultSpeed > speed.Value)
                {
                    ecb.SetSharedComponent(carEntity, acceleratingRenderMesh);
                }
                else
                {
                    ecb.SetSharedComponent(carEntity, defaultSpeedRenderMesh);
                }
            }).Run();
    }
}
