using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;


[UpdateInGroup(typeof(InitializationSystemGroup))]
public class BeeDyingSystem : SystemBase
{
    private EntityCommandBufferSystem m_CommandBufferSystem;

    protected override void OnCreate()
    {
        m_CommandBufferSystem = World.GetExistingSystem<EndInitializationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var ecb = m_CommandBufferSystem.CreateCommandBuffer();

        var deltaTime = Time.DeltaTime;

        var b = GetSingleton<BattleField>();

        Entities.WithAll<Dying>()
                .WithoutBurst()
                .ForEach( ( Entity bee, in Translation translation) =>
            {
                
                //If the bee has reached the floor
                //UnityEngine.Debug.Log(translation.Value.y+" "+(-b.Bounds.y));
                if(translation.Value.y <= -b.Bounds.y/2)
                {
                    //UnityEngine.Debug.Log("BEE IS AGONYZING");
                    //Not dying anymore
                    ecb.RemoveComponent<Dying>( bee );

                    //Not to be affected by gravity system
                    ecb.RemoveComponent<Velocity>( bee );


                    ecb.AddComponent<Agony>( bee );

                    //Adding NonUniformScale because it's not there by default for some reason
                    ecb.AddComponent<NonUniformScale>( bee );
                    ecb.SetComponent<NonUniformScale>(bee, new NonUniformScale{Value = new float3(1f, 1f, 1f)});
                }

                
                
            } ).Run();
        m_CommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}
