using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class FireUpdateSystem : SystemBase
{
    private EntityCommandBufferSystem m_CommandBufferSystem;
    private FireConfiguration m_config;
    private float m_timer;
    
    protected override void OnCreate()
    {
        m_CommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }
    
    protected override void OnUpdate()
    {
        m_timer -= Time.DeltaTime;
        if (m_timer >= 0)
            return;
        
        m_timer = m_config.FireSimUpdateRate;

        var fireConfigEntity = GetSingletonEntity<FireConfiguration>();
        m_config = EntityManager.GetComponentData<FireConfiguration>(fireConfigEntity);

        var ecb = m_CommandBufferSystem.CreateCommandBuffer();
        var temperatures = GetComponentDataFromEntity<Temperature>(true);
        var config = m_config;

        // 1st pass: 
        Entities
            .WithReadOnly(temperatures)
            .ForEach((Entity entity, ref AddedTemperature addedTemp, in DynamicBuffer<FireGridNeighbor> neighborBuffer) =>
            {
                float tempChange = 0;
                for (int i = 0; i < neighborBuffer.Length; ++i)
                {
                    FireGridNeighbor neigh = neighborBuffer[i];
                    Temperature theirTemp = temperatures[neigh.Entity];
                    
                    // On fire?
                    if (theirTemp.Value >= config.FlashPoint)
                    {
                        // My temperature changes depending on my neighbors
                        tempChange += theirTemp.Value * config.HeatTransferRate;
                    }
                }
                
                // Add neighbors' accumulated temperatures to my own added temperature (next job adds this effectively)
                addedTemp.Value += tempChange;

            }).ScheduleParallel();
        
        // 2nd pass: add each cell's accumulated neighbor temperature
        Entities
            .ForEach((Entity entity, ref Translation t, ref Temperature temp, ref AddedTemperature addedTemp, ref FireColor fireColor) =>
            {
                temp.Value = math.min(addedTemp.Value + temp.Value, config.MaxTemperature);
                addedTemp.Value = 0;

                // Affect look & feel
                if (temp.Value > 0)
                {
                    // Position
                    var newTrans = t;
                    newTrans.Value.y = temp.Value * config.MaxFireHeight + config.Origin.y;
                    t = newTrans;
                    
                    // Color
                    if (temp.Value < config.FlashPoint)
                    {
                        fireColor.Value = math.lerp(config.DefaultColor, config.LowFireColor,
                            math.max(0f, temp.Value / config.FlashPoint));
                    }
                    else
                    {
                        fireColor.Value = math.lerp(config.LowFireColor, config.HigHFireColor,
                            math.min(1f, (temp.Value - config.FlashPoint) / (1f - config.FlashPoint) ));
                    }
                }
                
            }).ScheduleParallel();
        
        m_CommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}
