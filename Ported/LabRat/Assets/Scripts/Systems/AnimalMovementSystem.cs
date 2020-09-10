//#define DEBUGGABLE

using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class AnimalMovementSystem : SystemBase
{
    // Sequential tile index from grid position
    public static int TileKeyFromPosition(float2 position, BoardDimensions dimensions)
    {
        var iPos = new int2((position + BoardDimensions.TileOffset) / BoardDimensions.TileSize);
        var key = iPos.x + iPos.y * dimensions.TileCountX;
        
        // Safety clamp until we know the board is consistent
        key = math.clamp(key, 0, dimensions.TileCountTotal - 1);
        
        Debug.Assert(key >= 0 && key < dimensions.TileCountTotal);
        return key;
    }

    // 2D direction vector from direction byte
    static float2 VectorFromDirection(Direction.Attributes direction)
    {
        var mask = new int4((int)direction) == new int4(1, 2, 4, 8);
        var components = math.select(float4.zero, new float4(-1f, 1f, 1f, -1f), mask);
        return components.xy + components.zw;
    }

    // 2D direction vector from direction byte
    internal static float RadiansFromDirection(Direction.Attributes direction)
    {
        var vector = VectorFromDirection(direction);
        return math.atan2(vector.y, -vector.x) + math.PI;
    }

    // Collect tile entities into a flat array. This should probably come from the board generator in some shape or
    // form, but for now we'll just grab it at startup.
    void EnsureTileEntityGrid()
    {
        if(!EntityManager.HasComponent<GameStateInitialize>(m_GameStateEntity))
            return;

        // Dispose previous grid
        m_TileEntityGrid.Dispose();

        // Capture board dimensions
        var boardAuthoring = GetSingleton<BoardCreationAuthor>();
        m_BoardDimensions.TileCountX = boardAuthoring.SizeX;
        m_BoardDimensions.TileCountTotal = boardAuthoring.SizeX * boardAuthoring.SizeY;

        // Build a direct lookup structure for tile entities
        var tileEntityGrid = m_TileEntityGrid = new NativeArray<Entity>(m_BoardDimensions.TileCountTotal, Allocator.Persistent);
        var boardDimensions = m_BoardDimensions;
        Entities.WithName("CollectTiles")
            .WithAll<Tile>()
            .ForEach((Entity entity, in PositionXZ pos) => { tileEntityGrid[TileKeyFromPosition(pos.Value, boardDimensions)] = entity; })
            .ScheduleParallel();
    }

    public struct BoardDimensions
    {
        public const float TileSize = 1f;
        public const float TileOffset = TileSize / 2f;
        
        public int TileCountX;
        public int TileCountTotal;
    }

    public BoardDimensions Dimensions => m_BoardDimensions;
    
    EntityCommandBufferSystem m_EntityCommandBufferSystem;
    Entity m_GameStateEntity;
    
    BoardDimensions m_BoardDimensions;
    NativeArray<Entity> m_TileEntityGrid;

    protected override void OnCreate()
    {
        m_EntityCommandBufferSystem = World.GetExistingSystem<BeginInitializationEntityCommandBufferSystem>();
        m_GameStateEntity = EntityManager.CreateEntity(typeof(WantsGameStateTransitions));
#if UNITY_EDITOR
        EntityManager.SetName(m_GameStateEntity, "MovementGameState");
#endif
        
        m_TileEntityGrid = new NativeArray<Entity>(0, Allocator.Persistent);
    }

    protected override void OnDestroy()
    {
        m_TileEntityGrid.Dispose();
    }

    protected override void OnUpdate()
    {
        // TODO: We should probably care about actual size of animals for cases where you fall into a hole or hit a goal?

        EnsureTileEntityGrid();
        
        // TODO: Investigate: Is there an official way to limit max delta time step in DOTS time?
        var deltaTime = math.clamp(Time.DeltaTime, 0f, 1f / 10f);
        
        var tileComponentData = GetComponentDataFromEntity<Tile>(true);
        var posComponentData = GetComponentDataFromEntity<PositionXZ>(/*true*/);
        var tileEntityGrid = m_TileEntityGrid;
        var boardDimensions = m_BoardDimensions;
        
        var ecb = m_EntityCommandBufferSystem.CreateCommandBuffer()
#if !DEBUGGABLE
            .AsParallelWriter()
#endif
            ;

        // TODO: Bit of a messy workaround here to avoid complaints about aliasing. Since we need to read position from the tiles,
        //       and at the same time mutate positions of the animals, we'd like to accept position in our lambdas since we know
        //       these belong to different archetypes and won't alias.
        
        Entities.WithName("MovementAndCollision")
            .WithReadOnly(tileComponentData)
            /*.WithReadOnly(posComponentData)*/
            .WithNativeDisableParallelForRestriction(posComponentData)
            .WithAll<PositionXZ>()
            .WithNone<Falling>()
            .ForEach((Entity entity, int entityInQueryIndex, /*ref PositionXZ pos,*/ ref Direction direction, in Speed speed) =>
        {
            // TEMP Grab pos from component array
            var pos = posComponentData[entity];
            
            // Grab tile data underneath animal
            var tileKey = TileKeyFromPosition(pos.Value, boardDimensions);
            var tileEntity = tileEntityGrid[tileKey];
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
                            ecb.AddComponent<Falling>(entity);
#else
                            ecb.AddComponent<Falling>(entityInQueryIndex, entity);
#endif
                        }
                        else if (obstacleMask == (int)Tile.Attributes.Goal)
                        {
                            var player = GetComponent<TileOwner>(tileEntity).Value;
                            var currentScore = GetComponent<Score>(player).Value;

                            var isRat = HasComponent<RatTag>(entity);
                            var newScore = isRat ? currentScore + 1 : currentScore * 0.7;
                            
#if DEBUGGABLE
                            Debug.Log($"Hit goal. Record player score!");
                            ecb.SetComponent(player, new Score(){Value = (int)newScore});
                            ecb.DestroyEntity(entity);
#else
                            ecb.SetComponent(entityInQueryIndex, player, new Score(){Value = (int)newScore});
                            ecb.DestroyEntity(entityInQueryIndex, entity);
#endif
                        }
                    }
                }
            }
            
            // TEMP Write back pos to component array
            posComponentData[entity] = pos;
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
