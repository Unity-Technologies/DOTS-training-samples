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
                .ForEach( ( int entityInQueryIndex, Entity entity, in Translation translation) =>
            {
                //If the bee has reached the floor
                if(translation.Value.y <= floor)
                {
                    //UnityEngine.Debug.Log("BEE IS AGONYZING");
                    ecb.RemoveComponent<Dying>( entityInQueryIndex, entity );
                    ecb.AddComponent<ScaleOutAndDestroy>( entityInQueryIndex, entity );
                    ecb.RemoveComponent<Velocity>( entityInQueryIndex,entity );
                    ecb.RemoveComponent<Rotation>( entityInQueryIndex,entity );
                    ecb.SetComponent<NonUniformScale>( entityInQueryIndex, entity, new NonUniformScale{Value = new float3(1f, 0.1f, 1f)});
                }
            } ).ScheduleParallel();
        m_CommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}
