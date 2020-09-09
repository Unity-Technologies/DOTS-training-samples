using Unity.Entities;
using Unity.Transforms;

public class IdleBeeSystem : SystemBase
{
    private EntityQuery m_ResourceQuery;
    
    private EntityCommandBufferSystem m_ECBSystem;
    
    private int m_StartingCount = -1;
    private int m_Score = 0;

    protected override void OnCreate()
    {
        m_ResourceQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new[]
            {
                ComponentType.ReadOnly<Velocity>(),
                ComponentType.ReadOnly<Translation>(),
            },
            None = new []
            {
                ComponentType.ReadOnly<TeamA>(), 
                ComponentType.ReadOnly<TeamB>(),
            }
        });
        

        m_ECBSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }
    
    protected override void OnUpdate()
    {
        
        
        Entities.WithAll<Idle>().ForEach( ( Entity entity, in Speed speed, in Translation spawnerTranslation ) =>
        {
            
        } ).ScheduleParallel();
    }
}
