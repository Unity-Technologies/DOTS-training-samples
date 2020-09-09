using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;


[UpdateInGroup(typeof(InitializationSystemGroup))]
public class BeeSpawningSystem : SystemBase
{
    private EntityCommandBufferSystem m_CommandBufferSystem;
    
    protected override void OnCreate()
    {
        m_CommandBufferSystem = World.GetExistingSystem<EndInitializationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var ecb = m_CommandBufferSystem.CreateCommandBuffer();

        Entities.ForEach((Entity spawnerEntity, in BeeSpawner spawner, in Translation spawnerTranslation) =>
        {

            //TODO : spawn the food equally on both side of the axis
            bool isTeamA = HasComponent<TeamA>( spawnerEntity );
            if( isTeamA )
            {
                // make it as teamA
                for( int i = 0; i < spawner.Count; ++i )
                {
                    var instance = ecb.Instantiate(spawner.BeePrefab_TeamA);
                    ecb.SetComponent(instance, new Translation {Value = spawnerTranslation.Value});
                    ecb.AddComponent<Idle>( instance );
                }
            }
            else
            {
                // make it as teamB
                for( int i = 0; i < spawner.Count; ++i )
                {
                    var instance = ecb.Instantiate(spawner.BeePrefab_TeamB);
                    ecb.SetComponent(instance, new Translation {Value = spawnerTranslation.Value});
                    ecb.AddComponent<Idle>( instance );
                }
            }
            
            ecb.DestroyEntity(spawnerEntity);
        }).Schedule();
        m_CommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}