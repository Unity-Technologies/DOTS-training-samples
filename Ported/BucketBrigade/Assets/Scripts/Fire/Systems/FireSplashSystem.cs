using Unity.Entities;
using UnityEngine;

public struct Splash : IComponentData
{
}

public struct SplashGridNeighbor : IBufferElementData
{
    public Entity Entity;
    public int RowIndex;
    public int ColumnIndex;
}

public class FireSplashSystem : SystemBase
{
    private EntityCommandBufferSystem m_CommandBufferSystem;
    
    protected override void OnCreate()
    {
        m_CommandBufferSystem = World.GetExistingSystem<EndInitializationEntityCommandBufferSystem>();
    }
    
    protected override void OnUpdate()
    {
        var ecb = m_CommandBufferSystem.CreateCommandBuffer();

        var waterConfigEntity = GetSingletonEntity<WaterInitialization>();
        var waterConfig = EntityManager.GetComponentData<WaterInitialization>(waterConfigEntity);

        Entities
            .ForEach((Entity entity, ref Temperature temp, ref AddedTemperature addedTemp, in Splash splash, in DynamicBuffer<SplashGridNeighbor> neighborBuffer) =>
            {
                // Extinguish center cell
                addedTemp.Value = -waterConfig.coolingStrength;

                for (int i = 0; i < neighborBuffer.Length; ++i)
                {
                    SplashGridNeighbor neigh = neighborBuffer[i];

                    float dowseCellStrength =
                        1f / (Mathf.Abs( neigh.RowIndex * waterConfig.coolingStrength_falloff) +
                              Mathf.Abs(neigh.ColumnIndex * waterConfig.coolingStrength_falloff));

                        float difference = - ((waterConfig.coolingStrength * dowseCellStrength) * waterConfig.bucketCapacity);
                    
                        // Extinguish center cell in FireUpdateSystem via AddedTemperature
                        ecb.SetComponent(neigh.Entity, new AddedTemperature { Value = difference } );
                }
                
                ecb.RemoveComponent(entity, ComponentType.ReadWrite<Splash>());
                
            }).Schedule();
      
        m_CommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}
