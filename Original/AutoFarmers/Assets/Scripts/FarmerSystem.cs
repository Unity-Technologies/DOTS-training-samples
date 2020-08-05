using Unity.Entities;
using Unity.Mathematics;

public struct FarmerIdle : IComponentData {}

public struct FarmerSmash : IComponentData
{
    Entity rock;
}

public struct FarmerTill : IComponentData { }

public struct FarmerPlant : IComponentData { }

public struct FarmerCollect : IComponentData { }

public struct FarmerTarget : IComponentData
{
    public float2 target;
}

public class FarmerSystem : SystemBase
{
    private Random m_Random;
    private EntityCommandBufferSystem m_CmdBufSystem;

    protected override void OnCreate()
    {
        m_Random = new Random(0x1234567);
        m_CmdBufSystem = World.GetExistingSystem<EndInitializationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var ecb = m_CmdBufSystem.CreateCommandBuffer().AsParallelWriter();
        var random = m_Random;
        m_Random.NextFloat2Direction();


        // handle all idle farmers
        Entities.WithAll<FarmerIdle>().ForEach((int entityInQueryIndex, Entity e) =>
        {
            // transition for smash state
            var target = new FarmerTarget {  target = math.abs( random.NextFloat2Direction() ) * new float2(60, 60) };

            ecb.RemoveComponent<FarmerIdle>(entityInQueryIndex, e);
            ecb.AddComponent<FarmerSmash>(entityInQueryIndex, e);
            ecb.AddComponent<FarmerTarget>(entityInQueryIndex, e, target);
        }).ScheduleParallel();

        m_CmdBufSystem.AddJobHandleForProducer(Dependency);


        // move all entities
        var deltaT = Time.DeltaTime;
        Entities.WithNone<WorkerTeleport>().ForEach((int entityInQueryIndex, Entity e, ref Position2D pos, in FarmerTarget target) =>
        {
            float2 dir = target.target - pos.position;
            float sqlen  = math.lengthsq( dir );
            float vel    = deltaT * 10;
            float sqvel  = vel * vel;

            if( sqlen < sqvel )
            {
                pos.position = target.target;
                ecb.RemoveComponent<FarmerTarget>(entityInQueryIndex, e);
            }
            else
            {
                pos.position += dir / math.sqrt( sqlen ) * vel;
            }
        }).ScheduleParallel();

        m_CmdBufSystem.AddJobHandleForProducer(Dependency);

        // arrived at location, should smash now
        Entities.WithAll<FarmerSmash>().WithNone<FarmerTarget>().ForEach((int entityInQueryIndex, Entity e) =>
        {
            ecb.RemoveComponent<FarmerSmash>(entityInQueryIndex, e);
            ecb.AddComponent<FarmerIdle>(entityInQueryIndex, e);
        }).ScheduleParallel();

        m_CmdBufSystem.AddJobHandleForProducer(Dependency);
    }
}