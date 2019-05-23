using System;
using System.Threading;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Jobs.LowLevel.Unsafe;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace JumpTheGun
{
    [UpdateAfter(typeof(TerrainSystem))]
    public class CannonballSystem : JobComponentSystem
    {
        private const float PLAYER_RADIUS = 0.3f;
        private const float BLOCK_HEIGHT_MIN = 0.5f;

        //[BurstCompile] // TODO(@cort) Can't AddComponent in Burst yet
        struct UpdateCannonballJob : IJobForEachWithEntity<Translation, ArcState>
        {
            public float3 PlayerCenterPosition;
            public float ElapsedTime;
            public int2 TerrainSize;
            public bool EnableInvincibility;
            [NativeDisableParallelForRestriction]
            [DeallocateOnJobCompletion]
            public NativeArray<int> BlockHitCounts;
            [ReadOnly]
            public NativeArray<float> BlockHeights;
            [ReadOnly]
            public NativeArray<Entity> BlockEntities;
            public float BoxHeightDamage;
            public EntityCommandBuffer.Concurrent CommandBuffer;

            public unsafe void Execute(Entity entity, int entityIndex, ref Translation cannonballPos, ref ArcState arc)
            {
                float t = ElapsedTime - arc.StartTime;
                if (t >= 0 && t < arc.Duration)
                {
                    // Test for collisions with the player
                    const float COLLISION_DISTANCE_SQ =
                        (PLAYER_RADIUS + TankFireSystem.CANNONBALL_RADIUS) *
                        (PLAYER_RADIUS + TankFireSystem.CANNONBALL_RADIUS);
                    if (math.lengthsq(cannonballPos.Value - PlayerCenterPosition) < COLLISION_DISTANCE_SQ)
                    {
                        if (!EnableInvincibility)
                        {
                            // game over! add a NewGame entity to trigger the NewGameSystem to restart.
                            Entity gameOverEntity = CommandBuffer.CreateEntity(entityIndex);
                            CommandBuffer.AddComponent(entityIndex, gameOverEntity, new NewGameTag());
                        }
                    }
                    
                    // Get the current height along the parabola & update cannonball pos
                    float s = t / arc.Duration;
                    cannonballPos.Value = new float3(
                        math.lerp(arc.StartPos.x, arc.EndPos.x, s),
                        Parabola.Solve(arc.Parabola.x, arc.Parabola.y, arc.Parabola.z, s),
                        math.lerp(arc.StartPos.z, arc.EndPos.z, s)
                    );
                }
                else if (t >= arc.Duration)
                {
                    // Cannonball has reached the end of its path; damage the block it hit
                    // TODO(@cort): If the destination block has been damaged/lowered since the shot was fired, the shot will "land" in the air above the block. Original game does this too.
                    int2 boxCoords = TerrainSystem.PositionToBlockCoords(cannonballPos.Value.x, cannonballPos.Value.z, TerrainSize);
                    int boxIndex = TerrainSystem.BlockCoordsToIndex(boxCoords, TerrainSize);
                    Entity boxEntity = BlockEntities[boxIndex];
                    int* blockHitsPtr = (int*)BlockHitCounts.GetUnsafePtr() + boxIndex;
                    int newCount = Interlocked.Increment(ref *blockHitsPtr);
                    if (newCount == 1)
                    {
                        CommandBuffer.AddComponent(entityIndex, boxEntity, new UpdateBlockTransformTag());
                    }
                    // TODO(@cort): There is a (relatively benign) race condition here. There's no guarantee that the last job
                    //              to update the hit counts array will also write the last command that updates the BlockHeight component.
                    //              This means that if >1 cannonball hits a block in a single frame, some of the hits may not damage the block.
                    //              One solution would be commands that allow mutating component values rather than setting them to a value
                    //              known at record time. https://github.com/Unity-Technologies/dots/issues/632
                    float currentBoxHeight = BlockHeights[boxIndex];
                    CommandBuffer.SetComponent(entityIndex, boxEntity,
                        new BlockHeight
                        {
                            Value = math.max(BLOCK_HEIGHT_MIN, currentBoxHeight - newCount * BoxHeightDamage)
                        });
                    // destroy the cannonball
                    CommandBuffer.DestroyEntity(entityIndex, entity);
                }
            }
        }

        private EntityQuery _cannonballQuery;
        private Options _options;
        private TerrainSystem _terrain;
        private BeginSimulationEntityCommandBufferSystem _barrier;
        private PlayerPositionCacheSystem _playerPosCache;
        protected override void OnCreateManager()
        {
            _cannonballQuery = GetEntityQuery(new ComponentType[] {
                ComponentType.ReadWrite<Translation>(),
                ComponentType.ReadWrite<ArcState>(),
                ComponentType.ReadWrite<Scale>(),
            });
            _terrain = World.GetOrCreateSystem<TerrainSystem>();
            _barrier = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
            _playerPosCache = World.GetOrCreateSystem<PlayerPositionCacheSystem>();

        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            if (_options == null)
            {
                _options = GameObject.Find("Options").GetComponent<Options>();
            }
            
            inputDeps = JobHandle.CombineDependencies(inputDeps, _terrain.UpdateCacheJob);
            var blockHitCounts = new NativeArray<int>(_options.terrainSizeX * _options.terrainSizeZ, Allocator.TempJob);
            JobHandle outputJob = new UpdateCannonballJob
            {
                PlayerCenterPosition = _playerPosCache.PlayerPosition + new float3(0,PLAYER_RADIUS,0),
                ElapsedTime = Time.time,
                TerrainSize = new int2(_options.terrainSizeX, _options.terrainSizeZ),
                EnableInvincibility = _options.invincibility,
                BlockHeights = _terrain.CachedBlockHeights,
                BlockEntities = _terrain.BlockEntities,
                BlockHitCounts = blockHitCounts,
                BoxHeightDamage = _options.boxHeightDamage,
                CommandBuffer = _barrier.CreateCommandBuffer().ToConcurrent(),
            }.Schedule(_cannonballQuery, inputDeps);
            _barrier.AddJobHandleForProducer(outputJob);
            return outputJob;
        }
    }
}
