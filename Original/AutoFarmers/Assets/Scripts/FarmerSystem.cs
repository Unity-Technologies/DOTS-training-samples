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

public struct ReplaceTileTilled : IComponentData { }

[UpdateAfter(typeof(GameInitSystem))]
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

    private static void initTilling(ref EntityCommandBuffer.ParallelWriter ecb, int entityInQueryIndex, Entity e, float2 tillPosition)
    {
        var ftill = new FarmerTill();
        ftill.startpos = tillPosition;

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
        var tileMap = GameInitSystem.groundTiles;
        uint2 mapSize = GameInitSystem.mapSize;

        // gather ground tiles
        //var groundData = m_GroundQuery.ToComponentDataArrayAsync<GroundData>(Allocator.TempJob, out var ballTranslationsHandle);

        // handle all idle farmers
        Entities.WithAll<FarmerIdle>().WithReadOnly(tileMap).ForEach((int entityInQueryIndex, Entity e, in Position2D farmerPosition) =>
        {
            const int states = 3;
            int state = (int)math.round((random.NextDouble() * states));

            var targetPosition = new float2( random.NextFloat() * mapSize.x, random.NextFloat() * mapSize.y );

            if( state == 0 ) // till field
            {
                if (tileMap.TryGetValue((uint2)targetPosition, out var tile))
                {
                    if (!HasComponent<Disabled>(tile.empty))
                        FarmerSystem.initTilling(ref ecb, entityInQueryIndex, e, targetPosition);
                }
            }
            else if( state == 1 ) // smash rocks
            {
                // transition for smash state
                var target = new FarmerTarget {  target = targetPosition };

                // TODO: check if rocks
                ecb.SetComponent(entityInQueryIndex, e, new Color {  Value = new float4(1,0,0,1) });
                ecb.RemoveComponent<FarmerIdle>(entityInQueryIndex, e);
                ecb.AddComponent<FarmerSmash>(entityInQueryIndex, e);
                ecb.AddComponent<FarmerTarget>(entityInQueryIndex, e, target);
            }
            else if (state == 2) // Plant
            {
                if (tileMap.TryGetValue((uint2)farmerPosition.position, out var tile))
                {
                    if (!HasComponent<Disabled>(tile.tilled))
                    {
                        ecb.SetComponent(entityInQueryIndex, e, new Color {  Value = new float4(0,1,0,1) });
                        ecb.AddComponent<FarmerPlant>(entityInQueryIndex, e);
                    }
                }
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


        // arrived at location, should smash now
        Entities.WithAll<FarmerSmash>().WithNone<FarmerTarget>().ForEach((int entityInQueryIndex, Entity e) =>
        {
            ecb.RemoveComponent<FarmerSmash>(entityInQueryIndex, e);
            ecb.AddComponent<FarmerIdle>(entityInQueryIndex, e);
            ecb.SetComponent(entityInQueryIndex, e, new Color {  Value = new float4(1,1,0,1) });
        }).ScheduleParallel();

        m_CmdBufSystem.AddJobHandleForProducer(Dependency);

        int tillFarmerCount = GetEntityQuery(typeof(FarmerTill)).CalculateEntityCount();

        // arrived at location, should till now
        Entities.WithAll<FarmerTill>().WithReadOnly(tileMap).WithNone<FarmerTarget>().ForEach((int entityInQueryIndex, Entity e, in Position2D position) =>
        {
            ecb.RemoveComponent<FarmerTill>(entityInQueryIndex, e);
            ecb.AddComponent<FarmerIdle>(entityInQueryIndex, e);
            ecb.SetComponent(entityInQueryIndex, e, new Color {  Value = new float4(1,1,0,1) });

            if (tileMap.TryGetValue((uint2)position.position, out var tile))
            {
                if (HasComponent<Position2D>(tile.empty))
                    ecb.AddComponent<Disabled>(entityInQueryIndex, tile.empty);
                    // ecb.AddComponent<ReplaceTileTilled>(entityInQueryIndex, tile.empty);
                ecb.RemoveComponent<Disabled>(entityInQueryIndex, tile.tilled);
            }

        }).ScheduleParallel();

        var q = GetEntityQuery(typeof(GroundTilledState), typeof(Position2D));
        // arrived at location, should plant now
        Entities.WithAll<FarmerPlant>().WithNone<FarmerTarget>()
            .WithReadOnly(tileMap)
            .ForEach((int entityInQueryIndex, Entity e, in Position2D farmerPosition) =>
        {
            ecb.RemoveComponent<FarmerPlant>(entityInQueryIndex, e);
            ecb.AddComponent<FarmerIdle>(entityInQueryIndex, e);

            if (tileMap.TryGetValue((uint2)farmerPosition.position, out var tile))
            {
                if (HasComponent<Position2D>(tile.empty))
                    ecb.AddComponent<PlantedSeedTag>(entityInQueryIndex, tile.empty);
            }

            ecb.SetComponent(entityInQueryIndex, e, new Color {  Value = new float4(0,1,0,1) });
        }).ScheduleParallel();

        m_CmdBufSystem.AddJobHandleForProducer(Dependency);
    }
}