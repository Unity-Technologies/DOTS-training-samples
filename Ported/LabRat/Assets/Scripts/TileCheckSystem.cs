using System.Collections.Generic;
using System.Diagnostics;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;


public class TileCheckSystem : SystemBase
{
    private const float positionThreshold = 0.01f;

    private EntityCommandBufferSystem m_ECBSystem;
    private EntityQuery m_tilesWithWallsQuery;

    protected override void OnCreate()
    {
        base.OnCreate();

        m_ECBSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();

        EntityQueryDesc desc = new EntityQueryDesc
        {
            // Query only matches chunks with both Red and Green components.
            All = new ComponentType[] {typeof(Wall), typeof(Translation) }
        };
        m_tilesWithWallsQuery = EntityManager.CreateEntityQuery(desc);

    }

    protected override void OnUpdate()
    {
        var ecb = m_ECBSystem.CreateCommandBuffer().AsParallelWriter();

        var walls = m_tilesWithWallsQuery.ToComponentDataArray<Wall>(Allocator.TempJob);
        var tileTranslations = m_tilesWithWallsQuery.ToComponentDataArray<Translation>(Allocator.TempJob);

        Entities
            .WithAll<TileCheckTag>()
            .ForEach((Entity entity, int entityInQueryIndex, ref Direction direction, ref Position position ) =>
        {
            for (int i = 0; i < tileTranslations.Length; i++)
            {
                if (!IsPositionCloseToTileCenter(position, tileTranslations[i], positionThreshold))
                {
                    //Not correct tile.
                    continue;
                }

                if ((direction.Value & walls[i].Value) == 0)
                {
                    //Direction wont collide with tile walls
                    continue;
                }

                switch (direction.Value)
                {
                    case DirectionDefines.North:
                        direction.Value = DirectionDefines.East;
                        break;
                    case DirectionDefines.South:
                        direction.Value = DirectionDefines.West;
                        break;
                    case DirectionDefines.East:
                        direction.Value = DirectionDefines.South;
                        break;
                    case DirectionDefines.West:
                        direction.Value = DirectionDefines.North;
                        break;
                    default:
                        break;
                }

                //position.Value = new float2(tileTranslations[i].Value.x, tileTranslations[i].Value.z);
                //UnityEngine.Debug.Log("New Direction: " + direction.Value);
            }

            //if we dont hit a wall, we still want to remove the tag regardless.
            ecb.RemoveComponent<TileCheckTag>(entityInQueryIndex, entity);
        }).ScheduleParallel();

        m_ECBSystem.AddJobHandleForProducer(Dependency);

        walls.Dispose(Dependency);
        tileTranslations.Dispose(Dependency);
    }

    static bool IsPositionCloseToTileCenter(Position position, Translation tileTranslation, float threshold)
    {
        return (math.abs(position.Value.x - tileTranslation.Value.x) <= threshold) &&
               (math.abs(position.Value.y - tileTranslation.Value.z) <= threshold);
    }
}
