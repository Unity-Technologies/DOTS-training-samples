using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;


[UpdateInGroup(typeof(InitializationSystemGroup))]
public class BeeAgonizingSystem : SystemBase
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

        Entities.WithAll<Agony>()
                .WithoutBurst()
                .ForEach( ( Entity bee, ref Scale scale) =>
            {
                
                float delta = 0.01f;
                scale.Value -= delta; 
                scale.Value = math.clamp(scale.Value, 0, 1);

                if(scale.Value <= 0)
                {
                    //ecb.DestroyEntity( bee );
                }


            } ).Run();
    }
}
