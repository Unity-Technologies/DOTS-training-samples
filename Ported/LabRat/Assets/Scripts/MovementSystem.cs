using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class MovementSystem : SystemBase
{
    private EntityCommandBufferSystem m_ECBSystem;
    private EntityQuery m_HolePositionQuery;

    private const float RoundingThreshold = 0.99f;

    protected override void OnCreate()
    {
        m_ECBSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();

        EntityQueryDesc desc = new EntityQueryDesc
        {
            All = new ComponentType[] {typeof(Hole), typeof(Translation)}
        };
        m_HolePositionQuery = EntityManager.CreateEntityQuery(desc);
    }

    protected override void OnUpdate()
    {
        var deltaTime = Time.DeltaTime;
        var ecb = m_ECBSystem.CreateCommandBuffer().AsParallelWriter();

        var holeTranslations = m_HolePositionQuery.ToComponentDataArray<Translation>(Allocator.TempJob);

        Entities.WithNone<TileCheckTag>().WithNone<Falling>().ForEach(
            (Entity entity, int entityInQueryIndex, ref Position position, ref Translation translation,
                ref TileCoord tileCoord, in Speed speed, in Direction direction) =>
            {
                var forward = float2.zero;
                //Convert direction to forward
                if ((direction.Value & DirectionDefines.North) == 1)
                {
                    forward = new float2(0, 1);
                }
                else if ((direction.Value & DirectionDefines.South) == 2)
                {
                    forward = new float2(0, -1);
                }
                else if ((direction.Value & DirectionDefines.East) == 4)
                {
                    forward = new float2(1, 0);
                }
                else if ((direction.Value & DirectionDefines.West) == 8)
                {
                    forward = new float2(-1, 0);
                }

                var prevTileX = tileCoord.Value.x;
                var prevTileY = tileCoord.Value.y;

                //Add direction * speed * deltaTime to position
                var deltaX = math.mul(math.mul(forward.x, speed.Value), deltaTime);
                var deltaY = math.mul(math.mul(forward.y, speed.Value), deltaTime);
                position.Value += new float2(deltaX, deltaY);

                var tileCenterOffset = position.Value - tileCoord.Value;

                bool fellIntoHole = false;
                for (int i = 0; i < holeTranslations.Length; i++)
                {
                    if (math.distancesq(holeTranslations[i].Value.x, position.Value.x) < 0.02 &&
                        math.distancesq(holeTranslations[i].Value.z, position.Value.y) < 0.02)
                    {
                        //Add Falling Tag
                        ecb.AddComponent<Falling>(entityInQueryIndex, entity);
                        fellIntoHole = true;
                    }
                }

                if (!fellIntoHole && (math.abs(tileCenterOffset.x) > 1) || math.abs(tileCenterOffset.y) > 1)
                {
                    //Add Tile Check Tag
                    ecb.AddComponent<TileCheckTag>(entityInQueryIndex, entity);
                    tileCoord.Value = new int2((int) (prevTileX + math.trunc(tileCenterOffset.x)),
                        (int) (prevTileY + math.trunc(tileCenterOffset.y)));
                }

                translation.Value = new float3(position.Value.x, 0, position.Value.y);
            }).ScheduleParallel();

        m_ECBSystem.AddJobHandleForProducer(Dependency);
        holeTranslations.Dispose(Dependency);
    }

    protected override void OnDestroy()
    {
        m_HolePositionQuery.Dispose();
    }
}