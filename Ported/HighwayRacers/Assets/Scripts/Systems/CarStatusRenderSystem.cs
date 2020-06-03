using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

public class CarStatusRenderSystem : SystemBase
{
    bool m_InitDone = false;
    RenderMesh m_BlockedRenderMesh;
    RenderMesh m_DefaultSpeedRenderMesh;
    RenderMesh m_AcceleratingRenderMesh;

    EntityCommandBufferSystem m_EntityCommandBufferSystem;

    protected override void OnCreate()
    {
        m_EntityCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        if (!m_InitDone)
        {
            m_InitDone = true;

            CarConfigurations carConfig = GetSingleton<CarConfigurations>();
            var carPrefab = carConfig.CarPrefab;

            m_BlockedRenderMesh = EntityManager.GetSharedComponentData<RenderMesh>(carPrefab);
            m_BlockedRenderMesh.material = CarStatusDisplayManager.Instance.BlockedStatusMaterial;

            m_DefaultSpeedRenderMesh = m_BlockedRenderMesh;
            m_DefaultSpeedRenderMesh.material = CarStatusDisplayManager.Instance.DefaultSpeedStatusMaterial;

            m_AcceleratingRenderMesh = m_BlockedRenderMesh;
            m_AcceleratingRenderMesh.material = CarStatusDisplayManager.Instance.AccelerationStatusMaterial;
        }
        
        var query = GetEntityQuery(ComponentType.ReadOnly<BlockedState>());
        EntityManager.SetSharedComponentData(query, m_BlockedRenderMesh);

        var ecb = m_EntityCommandBufferSystem.CreateCommandBuffer();

        var defaultSpeedRenderMesh = m_DefaultSpeedRenderMesh;
        var acceleratingRenderMesh = m_AcceleratingRenderMesh;

        Entities
            .WithoutBurst()
            .WithNone<BlockedState>()
            .ForEach((Entity carEntity, in CarProperties carProperty, in Speed speed) =>
            {
                if (carProperty.DefaultSpeed <= speed.Value)
                {
                    ecb.SetSharedComponent(carEntity, defaultSpeedRenderMesh);
                }
                else
                {
                    ecb.SetSharedComponent(carEntity, acceleratingRenderMesh);
                }
            }).Run();
    }
}
