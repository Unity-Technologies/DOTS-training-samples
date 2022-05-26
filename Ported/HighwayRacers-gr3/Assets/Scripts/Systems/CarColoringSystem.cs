using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using static Unity.Entities.SystemAPI;

[BurstCompile]
partial struct CarColoringSystem : ISystem
{
    private EntityQuery m_BaseColorQuery;

    public void OnDestroy(ref SystemState state)
    {
    }

    public void OnCreate(ref SystemState state)
    {
        m_BaseColorQuery = state.GetEntityQuery(typeof(URPMaterialPropertyBaseColor));
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var queryMask = m_BaseColorQuery.GetEntityQueryMask();

        var ecbSingleton = GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        foreach (var car in Query<CarAspect>())
        {
            float4 color = new float4();

            if (car.CurrentSpeed < car.CruisingSpeed)
            {
                color = new float4(1, 0, 0, 1);
            }
            else if (car.CurrentSpeed == car.CruisingSpeed)
            {
                color = new float4(.75f, .75f, .75f, 1);
            }
            else
            {
                color = new float4(0, 0, 1, 1);
            }

            ecb.SetComponentForLinkedEntityGroup(car.carEntity, queryMask, new URPMaterialPropertyBaseColor { Value = color });
        }
    }
}
