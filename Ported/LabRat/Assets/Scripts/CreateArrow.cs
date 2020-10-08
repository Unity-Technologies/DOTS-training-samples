using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class CreateArrow : SystemBase
{
    EntityCommandBufferSystem m_ECBSystem;
    private EntityQuery m_ArrowPositionQuery;

    protected override void OnCreate()
    {
        m_ECBSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        
        EntityQueryDesc desc = new EntityQueryDesc
        {
            All = new ComponentType[] {typeof(Arrow)}
        };
        m_ArrowPositionQuery = EntityManager.CreateEntityQuery(desc);
    }

    protected override void OnUpdate()
    {
        var arrowsEntities = m_ArrowPositionQuery.ToEntityArray(Allocator.TempJob);
        var arrowsPositions = m_ArrowPositionQuery.ToComponentDataArray<Arrow>(Allocator.TempJob);
        
        var boardSize = GetSingleton<GameInfo>().boardSize;
        var prefab = GetSingleton<GameInfo>().ArrowPrefab;

        var ecb = m_ECBSystem.CreateCommandBuffer().AsParallelWriter();

        Entities.ForEach((Entity e, int entityInQueryIndex, ref DynamicBuffer<PlayerArrow> arrows, in PlayerTransform playerIndex, in PlacingArrow placing) =>
        {
            float2 tilePosition = new float2(placing.TileIndex % boardSize.y, placing.TileIndex / boardSize.y );
            int tileIndex = placing.TileIndex;
            for (int j = 0; j < arrowsEntities.Length; j++)
            {
                // are we clicking on an existing arrow?
                if (arrowsPositions[j].Position == tileIndex)
                {
                    // is it one of ours?
                    for (int i = 0; i < arrows.Length; i++)
                    {
                        if (tileIndex == arrows[i].TileIndex)
                        {
                            ecb.DestroyEntity(entityInQueryIndex, arrowsEntities[j]);
                            arrows.RemoveAt(i);
                        }
                    }
                    return;
                }
            }
            // if we reach our maximum arrow, delete the first one
            if (arrows.Length == 3)
            {
                for (int j = 0; j < arrowsEntities.Length; j++)
                {
                    if (arrowsPositions[j].Position == arrows[0].TileIndex)
                    {
                        ecb.DestroyEntity(entityInQueryIndex, arrowsEntities[j]);
                    }
                }
                arrows.RemoveAt(0);
            }

            // create a new arrow
            var entity = ecb.Instantiate(entityInQueryIndex, prefab);
            ecb.SetComponent(entityInQueryIndex, entity, new Translation() {Value = new float3(tilePosition.x, 0.61f, tilePosition.y)});
            ecb.SetComponent(entityInQueryIndex, entity, PlayerUtility.ColorFromPlayerIndex(playerIndex.Index));
            ecb.SetComponent(entityInQueryIndex, entity, new Arrow() {Position = tileIndex});
            ecb.SetComponent(entityInQueryIndex, entity, new Direction() {Value = placing.Direction});
            Rotation rot;
            switch (placing.Direction)
            {
                case DirectionDefines.East:
                    rot = new Rotation(){Value = quaternion.EulerXYZ(math.radians(90), 0, 0)};
                    break;
                case DirectionDefines.North:
                    rot = new Rotation(){Value = quaternion.EulerXYZ(math.radians(90), math.radians(90), 0)};
                    break;
                case DirectionDefines.West:
                    rot = new Rotation(){Value = quaternion.EulerXYZ(math.radians(90), math.radians(180), 0)};
                    break;
                default: //DirectionDefines.South
                    rot = new Rotation(){Value = quaternion.EulerXYZ(math.radians(90), math.radians(-90), 0)};
                    break;
            }
            ecb.SetComponent(entityInQueryIndex, entity, rot);
            
            arrows.Add(new PlayerArrow() { TileIndex = tileIndex });
            ecb.RemoveComponent<PlacingArrow>(entityInQueryIndex, e);
        }).ScheduleParallel();
        
        m_ECBSystem.AddJobHandleForProducer(Dependency);
        var a = arrowsEntities.Dispose(Dependency);
        var b = arrowsPositions.Dispose(Dependency);
        Dependency = JobHandle.CombineDependencies(a, b);
    }
}
