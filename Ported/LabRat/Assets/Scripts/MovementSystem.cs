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
            (Entity entity, int entityInQueryIndex, ref Position position, ref Translation translation, in Speed speed,
                in Direction direction) =>
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

                var prevTileX = (int) math.round(position.Value.x - RoundingThreshold + 0.5);
                var prevTileY = (int) math.round(position.Value.y - RoundingThreshold + 0.5);

                //Add direction * speed * deltaTime to position
                var deltaX = math.mul(math.mul(forward.x, speed.Value), deltaTime);
                var deltaY = math.mul(math.mul(forward.y, speed.Value), deltaTime);
                position.Value += new float2(deltaX, deltaY);

                var newTileX = (int) math.round(position.Value.x - RoundingThreshold + 0.5);
                var newTileY = (int) math.round(position.Value.y - RoundingThreshold + 0.5);

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

                if (!fellIntoHole && (prevTileX != newTileX || prevTileY != newTileY))
                {
                    //Add Tile Check Tag
                    ecb.AddComponent<TileCheckTag>(entityInQueryIndex, entity);
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