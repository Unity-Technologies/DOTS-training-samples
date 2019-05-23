using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Jobs.LowLevel.Unsafe;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace JumpTheGun
{
    public class TerrainSystem : JobComponentSystem
    {
        public const float BOX_SPACING = 1.0f;

        public struct AABB
        {
            public float3 Min;
            public float3 Max;
        }

        public int2 TerrainSize { get; private set; }
        // Any systems that access the following arrays must use UpdateCacheJob to wait for them to be populated.
        public NativeArray<float> CachedBlockHeights;
        public NativeArray<byte> CachedIsBlockOccupied;
        public NativeArray<AABB> CachedBlockBoundingBoxes;
        public NativeArray<Entity> BlockEntities;
        // JobHandle to wait on to ensure safe access to the above arrays.
        public JobHandle UpdateCacheJob { get; private set; }

        public static int PositionToBlockIndex(float x, float z, int2 terrainSize)
        {
            int2 coords = PositionToBlockCoords(x, z, terrainSize);
            return BlockCoordsToIndex(coords, terrainSize);
        }

        public static int BlockCoordsToIndex(int2 coords, int2 terrainSize)
        {
            // TODO(@cort): morton curve? Hilbert curve?
            int blockIndex = (int)coords.y * terrainSize.x + (int)coords.x;
            return blockIndex;
        }

        public static int2 PositionToBlockCoords(float x, float z, int2 terrainSize)
        {
            return new int2((int)(x+0.5f), (int)(z+0.5f));
        }

        public static float3 BlockCoordsToPosition(int2 coords, float height, int2 terrainSize)
        {
            return new float3(coords.x, height, coords.y);
        }
        
        // Jobs to generate terrain & cache data from scratch
        //[BurstCompile] // TODO(@cort): Burst doesn't support SetComponent yet
        struct GenerateTerrainJob : IJobParallelFor
        {
            public EntityCommandBuffer.Concurrent CommandBuffer;
            public Entity BlockPrefab;
            public int2 TerrainSize;
            public float TerrainHeightMin;
            public float TerrainHeightMax;

            [NativeDisableParallelForRestriction]
            public NativeArray<Random> ThreadRngs;

#pragma warning disable 649
            [NativeSetThreadIndex]
            private int _threadIndex;
#pragma warning restore 649

            public void Execute(int jobIndex)
            {
                var rng = ThreadRngs[_threadIndex];
                float blockHeight = rng.NextFloat(TerrainHeightMin, TerrainHeightMax);
                ThreadRngs[_threadIndex] = rng;
                int2 blockCoords = new int2(jobIndex % TerrainSize.x, jobIndex / TerrainSize.x);
                int blockIndex = BlockCoordsToIndex(blockCoords, TerrainSize);
                float3 blockPosition = BlockCoordsToPosition(blockCoords, blockHeight, TerrainSize);
                Entity e = CommandBuffer.Instantiate(jobIndex, BlockPrefab);
                // TODO(@cort): should store e in BlockEntities yet, but it won't be valid (=non-negative) until the command buffer is played back.
                CommandBuffer.SetComponent(jobIndex, e, new BlockPositionXZ { Value = blockPosition.xz });
                CommandBuffer.SetComponent(jobIndex, e, new BlockIndex {Value = blockIndex});
                CommandBuffer.SetComponent(jobIndex, e, new BlockHeight {Value = blockHeight});
            }
        }
        [BurstCompile]
        struct GenerateTerrainCacheJob : IJobForEachWithEntity<BlockPositionXZ, BlockIndex, BlockHeight>
        {
            [NativeDisableParallelForRestriction]
            public NativeArray<float> BlockHeights;
            [NativeDisableParallelForRestriction]
            public NativeArray<AABB> BlockBoundingBoxes;
            [NativeDisableParallelForRestriction]
            public NativeArray<Entity> BlockEntities;

            public void Execute(Entity e, int entityIndex, [ReadOnly] ref BlockPositionXZ blockPos, [ReadOnly] ref BlockIndex blockIndex, [ReadOnly] ref BlockHeight height)
            {
                BlockHeights[blockIndex.Value] = height.Value;
                BlockEntities[blockIndex.Value] = e;
                BlockBoundingBoxes[blockIndex.Value] = new AABB
                {
                    Min = new float3(blockPos.Value.x - (BOX_SPACING/2), 0, blockPos.Value.y - (BOX_SPACING/2)),
                    Max = new float3(blockPos.Value.x + (BOX_SPACING/2), height.Value, blockPos.Value.y + (BOX_SPACING/2)),
                };
            }
        }
        [BurstCompile]
        struct MarkOccupiedBlocksJob : IJobForEach<Translation, TankBaseTag>
        {
            public int2 TerrainSize;
            [NativeDisableParallelForRestriction]
            public NativeArray<byte> IsBlockOccupied;

            public void Execute([ReadOnly] ref Translation position, [ReadOnly] ref TankBaseTag _)
            {
                int blockIndex = PositionToBlockIndex(position.Value.x, position.Value.z, TerrainSize);
                IsBlockOccupied[blockIndex] = 1;
            }
        }
        
        // Job to incrementally update the cached block heights for only blocks whose height changed. 
        [BurstCompile]
        struct UpdateTerrainCacheJob : IJobForEach<BlockPositionXZ, BlockIndex, BlockHeight, UpdateBlockTransformTag>
        {
            [NativeDisableParallelForRestriction]
            public NativeArray<float> BlockHeights;
            [NativeDisableParallelForRestriction]
            public NativeArray<AABB> BlockBoundingBoxes;

            public void Execute([ReadOnly] ref BlockPositionXZ blockPos, [ReadOnly] ref BlockIndex blockIndex, [ReadOnly] ref BlockHeight height, [ReadOnly] ref UpdateBlockTransformTag _)
            {
                BlockHeights[blockIndex.Value] = height.Value;
                BlockBoundingBoxes[blockIndex.Value] = new AABB
                {
                    Min = new float3(blockPos.Value.x - (BOX_SPACING/2), 0, blockPos.Value.y - (BOX_SPACING/2)),
                    Max = new float3(blockPos.Value.x + (BOX_SPACING/2), height.Value, blockPos.Value.y + (BOX_SPACING/2)),
                };
            }
        }

        public void Reset()
        {
            if (CachedBlockHeights.IsCreated)
            {
                CachedBlockHeights.Dispose();
            }
            if (CachedIsBlockOccupied.IsCreated)
            {
                CachedIsBlockOccupied.Dispose();
            }
            if (CachedBlockBoundingBoxes.IsCreated)
            {
                CachedBlockBoundingBoxes.Dispose();
            }
            if (BlockEntities.IsCreated)
            {
                BlockEntities.Dispose();
            }
            
            if (_options == null)
            {
                _options = GameObject.Find("Options").GetComponent<Options>();
            }

            // TODO(@cort): Need to differentiate between "terrain needs to be generated" and "terrain exists but cached arrays need to be repopulated from scratch"
            if (!CachedBlockHeights.IsCreated)
            {
                TerrainSize = new int2(_options.terrainSizeX, _options.terrainSizeZ);
                // Generate terrain
                var cmds = new EntityCommandBuffer(Allocator.TempJob);
                EntityArchetype blockArchetype = EntityManager.CreateArchetype(
                    typeof(BlockPositionXZ), typeof(BlockIndex), typeof(BlockHeight), typeof(LocalToWorld),
                    typeof(UpdateBlockTransformTag), typeof(RenderMesh));
                Entity blockPrefabEntity = EntityManager.CreateEntity(blockArchetype);
                EntityManager.SetSharedComponentData(blockPrefabEntity, _options.blockLook.Value);
                BlockEntities = new NativeArray<Entity>(TerrainSize.x * TerrainSize.y, Allocator.Persistent);
                var generateTerrainJob = new GenerateTerrainJob
                {
                    CommandBuffer = cmds.ToConcurrent(),
                    BlockPrefab = blockPrefabEntity,
                    TerrainSize = TerrainSize,
                    TerrainHeightMin = _options.terrainHeightMin,
                    TerrainHeightMax = _options.terrainHeightMax,
                    ThreadRngs = _threadRngs,
                };
                generateTerrainJob.Schedule(TerrainSize.x * TerrainSize.y, 64).Complete();
                cmds.Playback(EntityManager);
                cmds.Dispose();
                // Generate terrain caches
                CachedBlockHeights = new NativeArray<float>(TerrainSize.x * TerrainSize.y, Allocator.Persistent);
                CachedBlockBoundingBoxes =
                    new NativeArray<AABB>(TerrainSize.x * TerrainSize.y, Allocator.Persistent);
                var generateTerrainCacheJob = new GenerateTerrainCacheJob
                {
                    BlockHeights = CachedBlockHeights,
                    BlockBoundingBoxes = CachedBlockBoundingBoxes,
                    BlockEntities = BlockEntities,
                };
                generateTerrainCacheJob.Schedule(this).Complete();
                // Place tanks and player entities
                const int playerCount = 1;
                int tankCount = math.min(_options.tankCount, (_options.terrainSizeX * _options.terrainSizeZ - playerCount));
                int spawnCount = tankCount + playerCount;
                EntityArchetype playerArchetype =
                    EntityManager.CreateArchetype(typeof(Translation), typeof(ArcState), typeof(LocalToWorld), typeof(RenderMesh));
                EntityArchetype tankBaseArchetype =
                    EntityManager.CreateArchetype(typeof(Translation), typeof(Rotation), typeof(LocalToWorld), typeof(RenderMesh),
                        typeof(TankBaseTag));
                EntityArchetype tankCannonArchetype =
                    EntityManager.CreateArchetype(typeof(Translation), typeof(Rotation), typeof(LocalToWorld), typeof(RenderMesh),
                        typeof(TankFireCountdown), typeof(TankCannonTag), typeof(LocalToParent), typeof(Parent));
                int lfsr = UnityEngine.Random.Range(1, 65536);
                int bit = 0;
                int validOutputCount = _options.terrainSizeX * _options.terrainSizeZ;
                int2 terrainSize = new int2(_options.terrainSizeX, _options.terrainSizeZ);
                for (int iSpawn = 0; iSpawn < spawnCount; ++iSpawn)
                {
                    // linear feedback shift register
                    // taps: 16 15 13 4; feedback polynomial: x^16 + x^15 + x^13 + x^4 + 1
                    // period: 65535 (suitable for up to 255*255 terrain)
                    int next = -1;
                    do
                    {
                        bit = ((lfsr >> 0) ^ (lfsr >> 1) ^ (lfsr >> 3) ^ (lfsr >> 12)) & 1;
                        lfsr = (lfsr >> 1) | (bit << 15);
                        if (lfsr - 1 < validOutputCount)
                            next = lfsr - 1;
                    } while (next < 0);
    
                    // Extract x,y coords from outputValue and spawn a Thing there.
                    int z = next / _options.terrainSizeX;
                    int x = next % _options.terrainSizeX;
                    int blockIndex = PositionToBlockIndex(x, z, terrainSize);
                    float blockHeight = CachedBlockHeights[blockIndex];
                    if (iSpawn < playerCount)
                    {
                        // Create player
                        Entity player = EntityManager.CreateEntity(playerArchetype);
                        EntityManager.SetSharedComponentData(player, _options.playerLook.Value);
                        float3 playerStartPos = new float3(x, blockHeight, z);
                        _playerPosCache.SetInitialPosition(playerStartPos);
                        EntityManager.SetComponentData(player, new Translation {Value = playerStartPos});
                        EntityManager.SetComponentData(player, new ArcState
                        {
                            StartTime = float.MinValue, // force a new bounce on the first frame
                        });
                    }
                    else
                    {
                        // Create tank base
                        Entity tankBase = EntityManager.CreateEntity(tankBaseArchetype);
                        EntityManager.SetSharedComponentData(tankBase, _options.tankBaseLook.Value);
                        EntityManager.SetComponentData(tankBase,
                            new Translation {Value = new float3(x, blockHeight, z)});
                        // Create tank cannon
                        const float TANK_CANNON_Y_OFFSET = 0.4f;
                        Entity tankCannon = EntityManager.CreateEntity(tankCannonArchetype);
                        EntityManager.SetSharedComponentData(tankCannon, _options.tankCannonLook.Value);
                        EntityManager.SetComponentData(tankCannon,
                            new Translation {Value = new float3(0, TANK_CANNON_Y_OFFSET, 0)});
                        EntityManager.SetComponentData(tankCannon,
                            new TankFireCountdown
                                {SecondsUntilFire = UnityEngine.Random.Range(0, _options.tankLaunchPeriod)});
                        EntityManager.SetComponentData(tankCannon, new Parent {Value = tankBase});
                    }
                }                
                // Generate a cache of blocks occupied by tanks
                CachedIsBlockOccupied = new NativeArray<byte>(TerrainSize.x * TerrainSize.y, Allocator.Persistent);
                var markOccupiedBlocksJob = new MarkOccupiedBlocksJob
                {
                    TerrainSize = TerrainSize,
                    IsBlockOccupied = CachedIsBlockOccupied,
                };
                markOccupiedBlocksJob.Schedule(this).Complete();
            }
        }

        protected override void OnDestroyManager()
        {
            _threadRngs.Dispose();
            if (CachedBlockHeights.IsCreated)
            {
                CachedBlockHeights.Dispose();
            }
            if (CachedIsBlockOccupied.IsCreated)
            {
                CachedIsBlockOccupied.Dispose();
            }
            if (CachedBlockBoundingBoxes.IsCreated)
            {
                CachedBlockBoundingBoxes.Dispose();
            }
            if (BlockEntities.IsCreated)
            {
                BlockEntities.Dispose();
            }
        }

        private NativeArray<Random> _threadRngs;
        private Options _options;
        private CannonballSystem _cannonballSystem;
        private TankFireSystem _tankFireSystem;
        private PlayerPositionCacheSystem _playerPosCache;
        protected override void OnCreateManager()
        {
            _cannonballSystem = World.GetOrCreateSystem<CannonballSystem>();
            _tankFireSystem = World.GetOrCreateSystem<TankFireSystem>();
            _playerPosCache = World.GetOrCreateSystem<PlayerPositionCacheSystem>();
            uint seederSeed = math.max(1, (uint)System.DateTime.Now.Ticks);
            Random rngSeeder = new Random(seederSeed);
            _threadRngs = new NativeArray<Random>(JobsUtility.MaxJobThreadCount, Allocator.Persistent);
            for(int i=0; i<_threadRngs.Length; ++i)
            {
                var rng = _threadRngs[i];
                rng.InitState(rngSeeder.NextUInt());
                _threadRngs[i] = rng;
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            UpdateCacheJob = new UpdateTerrainCacheJob
            {
                BlockHeights = CachedBlockHeights,
                BlockBoundingBoxes = CachedBlockBoundingBoxes,
            }.Schedule(this, inputDeps);
            return UpdateCacheJob;
        }
    }
}
