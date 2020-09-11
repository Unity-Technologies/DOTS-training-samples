using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class BeeDyingSystem : SystemBase
{
    private EntityCommandBufferSystem m_CommandBufferSystem;

    protected override void OnCreate()
    {
        m_CommandBufferSystem = World.GetExistingSystem<EndInitializationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var ecb = m_CommandBufferSystem.CreateCommandBuffer().AsParallelWriter();
        var b = GetSingleton<BattleField>();
        float floor = -b.Bounds.y / 2f;

        Entities.WithAll<Dying>()
                .ForEach( ( int entityInQueryIndex, Entity bee, in Translation translation) =>
            {
                //If the bee has reached the floor
                if(translation.Value.y <= floor)
                {
                    //UnityEngine.Debug.Log("BEE IS AGONYZING");
                    ecb.RemoveComponent<Dying>( entityInQueryIndex, bee );
                    ecb.RemoveComponent<Velocity>( entityInQueryIndex,bee );
                    ecb.AddComponent<Agony>( entityInQueryIndex, bee );

                    //Adding NonUniformScale because it's not there by default for some reason
                    //ecb.AddComponent<NonUniformScale>( bee );
                    ecb.SetComponent<NonUniformScale>( entityInQueryIndex, bee, new NonUniformScale{Value = new float3(1f, 1f, 1f)});
                }
            } ).ScheduleParallel();
        m_CommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}
