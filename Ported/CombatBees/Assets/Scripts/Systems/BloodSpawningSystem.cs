using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;


[UpdateInGroup(typeof(InitializationSystemGroup))]
public class BloodSpawningSystem : SystemBase
{
    private EntityCommandBufferSystem m_CommandBufferSystem;
    private Random m_Random;
    
    protected override void OnCreate()
    {
        m_CommandBufferSystem = World.GetExistingSystem<EndInitializationEntityCommandBufferSystem>();
        m_Random = new Random( 9 );
    }

    protected override void OnUpdate()
    {
        var random = new Random( (uint)m_Random.NextInt() );
        var ecb = m_CommandBufferSystem.CreateCommandBuffer();

        Entities.ForEach((Entity spawnerEntity, in BloodSpawner spawner, in Translation spawnerTranslation) =>
        {
            for( int i = 0; i < spawner.Count; ++i )
            {
                float randomVelocityX = random.NextFloat( -5f, 5f );
                float randomVelocityY = random.NextFloat( -5f, 5f );
                float randomVelocityZ = random.NextFloat( -5f, 5f );

                var instance = ecb.Instantiate(spawner.blood);
                ecb.SetComponent(instance, new Translation {Value = spawnerTranslation.Value});
                ecb.SetComponent(instance, new Velocity {Value = new float3(randomVelocityX, randomVelocityY, randomVelocityZ)});
                ecb.SetComponent( instance, new NonUniformScale{ Value = new float3( random.NextFloat( 0.25f, .75f ), random.NextFloat( 0.25f, .75f ), random.NextFloat( 0.25f, .75f ) )} );
            }
            
            ecb.DestroyEntity(spawnerEntity);
        }).Schedule();
        m_CommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}