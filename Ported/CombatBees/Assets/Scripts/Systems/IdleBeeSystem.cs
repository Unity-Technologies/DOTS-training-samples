using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Mathematics;

public class IdleBeeSystem : SystemBase
{
    private EntityQuery m_ResourceQuery;
    private EntityQuery m_TeamABees;
    private EntityQuery m_TeamBBees;
    
    private Random m_Random;
    private EntityCommandBufferSystem m_ECBSystem;
    
    protected override void OnCreate()
    {
        m_ResourceQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new[]
            {
                ComponentType.ReadOnly<Resource>(),
                ComponentType.ReadOnly<Translation>(),
            },
            None = new []
            {
                ComponentType.ReadOnly<Parent>(), 
            }
        });
        
        m_TeamABees = GetEntityQuery(new EntityQueryDesc
        {
            All = new[]
            {
                ComponentType.ReadOnly<TeamA>(),
                ComponentType.ReadOnly<Bee>(),
            },
            None = new []
            {
                ComponentType.ReadOnly<Dying>(), 
            }
        });
        
        m_TeamBBees = GetEntityQuery(new EntityQueryDesc
        {
            All = new[]
            {
                ComponentType.ReadOnly<TeamB>(),
                ComponentType.ReadOnly<Bee>(),
            }, None = new []
            {
                ComponentType.ReadOnly<Dying>(), 
            }
        });
        
        m_Random = new Random( 7 );
        m_ECBSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }
    
    protected override void OnUpdate()
    {
        var resourceEntities =
            m_ResourceQuery.ToEntityArrayAsync(Allocator.TempJob, out var resourcesEntitiesHandle);
        var beeEntities_TeamA =
            m_ResourceQuery.ToEntityArrayAsync(Allocator.TempJob, out var beeAEntitiesHandle);
        var beeEntities_TeamB =
            m_ResourceQuery.ToEntityArrayAsync(Allocator.TempJob, out var beeBEntitiesHandle);
        
        Dependency = JobHandle.CombineDependencies(Dependency, resourcesEntitiesHandle);
        Dependency = JobHandle.CombineDependencies(Dependency, beeAEntitiesHandle);
        Dependency = JobHandle.CombineDependencies(Dependency, beeBEntitiesHandle);
        
        var ecb = m_ECBSystem.CreateCommandBuffer();
        if( resourceEntities.Length == 0 )
        {
            // go attacking here
            Entities.WithAll<TeamA>()
                .WithDisposeOnCompletion( beeEntities_TeamB )
                .WithAll<Idle>()
                .ForEach( ( Entity bee ) =>
            {
                ecb.RemoveComponent<Idle>( bee );
                ecb.AddComponent<Attack>( bee );

                int targetIndex = m_Random.NextInt( 0, beeEntities_TeamB.Length );
                ecb.AddComponent( bee, new TargetEntity { Value = beeEntities_TeamB[targetIndex] } );
            } ).Schedule();
            
            Entities.WithAll<TeamB>()
                .WithDisposeOnCompletion( beeEntities_TeamA )
                .WithAll<Idle>()
                .ForEach( ( Entity bee ) =>
                {
                    ecb.RemoveComponent<Idle>( bee );
                    ecb.AddComponent<Attack>( bee );

                    int targetIndex = m_Random.NextInt( 0, beeEntities_TeamA.Length );
                    ecb.AddComponent( bee, new TargetEntity { Value = beeEntities_TeamA[targetIndex] } );
                } ).Schedule();
        }
        else
        {
            
            Entities.WithAll<Idle>().ForEach( ( Entity bee ) =>
            {
                ecb.RemoveComponent<Idle>( bee );
                ecb.AddComponent<Collecting>( bee );

                int targetIndex = m_Random.NextInt( 0, resourceEntities.Length );
                ecb.AddComponent( bee, new TargetEntity {Value = resourceEntities[targetIndex]} );
            } ).Schedule();
        }
        
        m_ECBSystem.AddJobHandleForProducer(Dependency);
    }
}
