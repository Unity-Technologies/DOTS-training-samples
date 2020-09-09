//#define DEBUGGABLE

using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class AnimalMovementSystem : SystemBase
{
    // Placeholder inputs for test-scene
    const int TileCount = 8;
    const int TileCountSqr = TileCount * TileCount;
    const float TileSize = 1f;
    const float TileOffset = TileSize / 2f;

    // Sequential tile index from grid position
    static int TileKeyFromPosition(float2 position)
    {
        var iPos = new int2((position + TileOffset) / TileSize);
        var key = iPos.x + iPos.y * TileCount;
        Debug.Assert(key >= 0 && key < TileCountSqr);
        return key;
    }

    // 2D direction vector from direction byte
    static float2 VectorFromDirection(Direction.Attributes direction)
    {
        var mask = new int4((int)direction) == new int4(1, 2, 4, 8);
        var components = math.select(float4.zero, new float4(-1f, 1f, 1f, -1f), mask);
        return components.xy + components.zw;
    }

    // Collect tile entities into a flat array. This should probably come from the board generator in some shape or
    // form, but for now we'll just grab it at startup.
    void EnsureTileEntityGrid()
    {
        if(m_TileEntityGrid.IsCreated)
            return;

        // Build a direct lookup structure for tile entities
        var tileEntityGrid = m_TileEntityGrid = new NativeArray<Entity>(TileCountSqr, Allocator.Persistent);
        Entities.WithAll<Tile>().ForEach((Entity entity, in PositionXZ pos) => { tileEntityGrid[TileKeyFromPosition(pos.Value)] = entity; }).Run();
    }

    EntityCommandBufferSystem m_EntityCommandBufferSystem;
    NativeArray<Entity> m_TileEntityGrid;

    protected override void OnCreate()
    {
        m_EntityCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnDestroy()
    {
        m_TileEntityGrid.Dispose();
    }

    protected override void OnUpdate()
    {
        // TODO: We should probably care about actual size of animals for cases where you fall into a hole or hit a goal?

        EnsureTileEntityGrid();
        
        var deltaTime = Time.DeltaTime;
        var tileComponentData = GetComponentDataFromEntity<Tile>(true);
        var posComponentData = GetComponentDataFromEntity<PositionXZ>(true);
        var tileEntityGrid = m_TileEntityGrid;
        
        var ecb = m_EntityCommandBufferSystem.CreateCommandBuffer()
#if !DEBUGGABLE
            .AsParallelWriter();
#endif
            ;

        Entities
            .WithReadOnly(tileComponentData)
            .WithReadOnly(posComponentData)
            .ForEach((Entity entity, int entityInQueryIndex, ref PositionXZ pos, ref Direction direction, in Speed speed) =>
        {
            // Grab tile data underneath animal
            var tileEntity = tileEntityGrid[TileKeyFromPosition(pos.Value)];
            var tileData = tileComponentData[tileEntity];

            // Calculate move data 
            var moveDirectionVec = VectorFromDirection(direction.Value);
            var moveDeltaVec = moveDirectionVec * speed.Value * deltaTime;
            var newPos = pos.Value + moveDeltaVec;

            // Masks based on tile attributes
            var wallDirectionMask = (int)tileData.Value & (int)direction.Value;
            var arrowMask = (int)(tileData.Value & Tile.Attributes.ArrowAny) >> (int)Tile.Attributes.ArrowShiftCount;
            var arrowDirectionMask = arrowMask & ~(int)direction.Value;
            var obstacleMask = (int)(tileData.Value & Tile.Attributes.ObstacleAny);
            
            if ((wallDirectionMask | arrowDirectionMask | obstacleMask) == 0)
            {
                // The current tile has nothing of interest and we can freely keep moving forward if:
                // - No walls are affecting our current direction of travel
                // - No arrows are affecting our current direction of travel
                // - We're not encountering a hole
                // - We're not encountering a goal
                pos.Value = newPos;
            }
            else
            {
                var tileCenter = posComponentData[tileEntity].Value;
                var oldPosToCenter = tileCenter - pos.Value;
                var newPosToCenter = tileCenter - newPos;

                if (math.dot(oldPosToCenter, newPosToCenter) > 0f)
                {
                    // We have not yet reached / already passed tile center, so we'll keep on going for now
                    pos.Value = newPos;
                }
                else
                {
                    // About to pass through the tile center; this is when we currently handle all logic
                    // (basic snap to turn point for now, we should correctly preserve travel distance when turning)
                    pos.Value = tileCenter;

                    if (arrowDirectionMask != 0)
                    {
#if DEBUGGABLE
                        Debug.Log($"Turning {newPos} from {direction.Value} to arrow {(Direction.Attributes)arrowDirectionMask}");
#endif
                        
                        direction.Value = (Direction.Attributes)arrowDirectionMask;
                    }

#if DEBUGGABLE
                    Debug.Log($"{entity} Turning {newPos} from {direction.Value} (tile has {tileData.Value})");
#endif

                    while (((int)direction.Value & (int)tileData.Value) != 0)
                        direction.Value = (Direction.Attributes)(((int)direction.Value << 1) % 0xF);
                    
#if DEBUGGABLE
                    Debug.Log($"..to {direction.Value}");
#endif

                    if (obstacleMask != 0)
                    {
                        if (obstacleMask == (int)Tile.Attributes.Hole)
                        {
#if DEBUGGABLE
                            Debug.Log($"Fell into hole. I am falling!");
                            ecb.RemoveComponent<PositionXZ>(entity);
                            ecb.AddComponent<Falling>(entity);
#else
                            ecb.RemoveComponent<PositionXZ>(entityInQueryIndex, entity);
                            ecb.AddComponent<Falling>(entityInQueryIndex, entity);
#endif
                        }
                        else if (obstacleMask == (int)Tile.Attributes.Goal)
                        {
#if DEBUGGABLE
                            Debug.Log($"Hit goal. Record player score!");
                            ecb.DestroyEntity(entity);
#else
                            ecb.DestroyEntity(entityInQueryIndex, entity);
#endif
                        }
                    }
                }
            }
        })
#if DEBUGGABLE
        .WithoutBurst()
        .Run();
#else
        .ScheduleParallel();
#endif
        
        m_EntityCommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}
