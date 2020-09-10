using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;


[UpdateInGroup(typeof(InitializationSystemGroup))]
public class SmokeSpawningSystem : SystemBase
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

        Entities.ForEach((Entity spawnerEntity, in SmokeSpawner spawner, in Translation spawnerTranslation) =>
        {

            for( int i = 0; i < spawner.Count; ++i )
            {

                float randomScale = random.NextFloat( 1f, 2f );
                float randomVelocityX = random.NextFloat( -15f, 15f );
                float randomVelocityY = random.NextFloat( -15f, 15f );
                float randomVelocityZ = random.NextFloat( -15f, 15f );

                var instance = ecb.Instantiate(spawner.Smoke);
                ecb.SetComponent(instance, new Translation {Value = spawnerTranslation.Value});
                ecb.SetComponent(instance, new Velocity {Value = new float3(randomVelocityX, randomVelocityY, randomVelocityZ)});
                ecb.AddComponent<NonUniformScale>( instance, new NonUniformScale{Value = new float3(randomScale, randomScale, randomScale)} );
                //Use the same state as the bee
                ecb.AddComponent<Agony>( instance );
            }
            
            ecb.DestroyEntity(spawnerEntity);
        }).Schedule();
        m_CommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}