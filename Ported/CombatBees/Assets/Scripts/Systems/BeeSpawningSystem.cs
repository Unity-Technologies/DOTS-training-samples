using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;


[UpdateInGroup(typeof(InitializationSystemGroup))]
public class BeeSpawningSystem : SystemBase
{
    private Random m_Random;
    
    protected override void OnCreate()
    {
        m_Random = new Random( 7 );
    }
    
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.TempJob);
        var random = new Random( (uint)m_Random.NextUInt() );

        // Spawn synchronously
        Entities.ForEach((Entity spawnerEntity, in BeeSpawner spawner, in Translation spawnerTranslation) =>
        {
            bool isTeamA = HasComponent<TeamA>( spawnerEntity );
            Entity beePrefab = isTeamA ? spawner.BeePrefab_TeamA : spawner.BeePrefab_TeamB;
            for( int i = 0; i < spawner.Count; ++i )
            {
                var instance = ecb.Instantiate(beePrefab);
                ecb.SetComponent(instance, new Translation {Value = spawnerTranslation.Value});
                ecb.AddComponent<Idle>( instance );
                ecb.AddComponent<TargetPosition>( instance, new TargetPosition { Value = float3.zero } );
                
                // apply initial direction and speed
                ecb.SetComponent<Velocity>( instance, new Velocity{ Value = random.NextFloat3Direction() * 30 });
            }
            ecb.DestroyEntity(spawnerEntity);
        }).Run();
        
        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}