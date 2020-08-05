using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;

public struct FarmerIdle : IComponentData {}

public struct FarmerSmash : IComponentData
{
    Entity rock;
}

public struct FarmerTill : IComponentData
{
    public float2 startpos;
    public int2 dimensions;
}

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
    private EntityQuery m_GroundQuery;

    protected override void OnCreate()
    {
        m_Random = new Random(0x1234567);
        m_CmdBufSystem = World.GetExistingSystem<EndInitializationEntityCommandBufferSystem>();
        m_GroundQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new[]
        {
                ComponentType.ReadOnly<GroundData>(),
            }
        });

    }

    private static void initTilling(ref EntityCommandBuffer.ParallelWriter ecb, int entityInQueryIndex, Entity e)
    {
        var ftill = new FarmerTill();
        ftill.startpos = new float2( 34, 22 );

        ecb.SetComponent(entityInQueryIndex, e, new Color {  Value = new float4(0,0,1,1) });
        ecb.RemoveComponent<FarmerIdle>(entityInQueryIndex, e);
        ecb.AddComponent<FarmerTill>(entityInQueryIndex, e);
        ecb.AddComponent<FarmerTarget>(entityInQueryIndex, e, new FarmerTarget { target = ftill.startpos } );
    }

    protected override void OnUpdate()
    {
        var ecb = m_CmdBufSystem.CreateCommandBuffer().AsParallelWriter();
        var random = m_Random;
        m_Random.NextFloat2Direction();

        // gather ground tiles
        //var groundData = m_GroundQuery.ToComponentDataArrayAsync<GroundData>(Allocator.TempJob, out var ballTranslationsHandle);


        // handle all idle farmers
        Entities.WithAll<FarmerIdle>().ForEach((int entityInQueryIndex, Entity e) =>
        {
            const int states = 2;
            int state = ((int) (random.NextDouble() * 2.0)) % states;

            if( state == 0 ) // till field
            {
                FarmerSystem.initTilling( ref ecb, entityInQueryIndex, e);
            }
            else if( state == 1 ) // smash rocks
            {
                // transition for smash state
                var target = new FarmerTarget {  target = math.abs( random.NextFloat2Direction() ) * new float2(60, 60) };

                ecb.SetComponent(entityInQueryIndex, e, new Color {  Value = new float4(1,0,0,1) });
                ecb.RemoveComponent<FarmerIdle>(entityInQueryIndex, e);
                ecb.AddComponent<FarmerSmash>(entityInQueryIndex, e);
                ecb.AddComponent<FarmerTarget>(entityInQueryIndex, e, target);
            }
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


        // arrived at location, should till now
        Entities.WithAll<FarmerSmash>().WithNone<FarmerTarget>().ForEach((int entityInQueryIndex, Entity e) =>
        {
            ecb.RemoveComponent<FarmerSmash>(entityInQueryIndex, e);
            ecb.AddComponent<FarmerIdle>(entityInQueryIndex, e);
            ecb.SetComponent(entityInQueryIndex, e, new Color {  Value = new float4(1,1,0,1) });
        }).ScheduleParallel();

        m_CmdBufSystem.AddJobHandleForProducer(Dependency);


        // arrived at location, should smash now
        Entities.WithAll<FarmerTill>().WithNone<FarmerTarget>().ForEach((int entityInQueryIndex, Entity e) =>
        {
            ecb.RemoveComponent<FarmerTill>(entityInQueryIndex, e);
            ecb.AddComponent<FarmerIdle>(entityInQueryIndex, e);
            ecb.SetComponent(entityInQueryIndex, e, new Color {  Value = new float4(1,1,0,1) });
        }).ScheduleParallel();

        m_CmdBufSystem.AddJobHandleForProducer(Dependency);
    }
}