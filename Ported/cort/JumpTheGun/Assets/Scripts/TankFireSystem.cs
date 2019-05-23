using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Assertions;

namespace JumpTheGun
{
    [UpdateAfter(typeof(TerrainSystem))]
    class TankFireSystem : JobComponentSystem
    {
        const float CANNONBALL_SPEED = 2.5f;
        public const float CANNONBALL_RADIUS = 0.25f;
        const float CANNONBALL_PARABOLA_PRECISION = 0.1f;
        const float COLLISION_STEP_MULTIPLIER = 3.0f;

        static float3 GetSimulatedPosition(float3 start, float3 end, float paraA, float paraB, float paraC, float t) {
            return new Vector3(
                math.lerp(start.x, end.x, t),
                Parabola.Solve(paraA, paraB, paraC, t),
                math.lerp(start.z, end.z, t)
            );
        }
        static private bool BoxHitsTerrain(NativeArray<TerrainSystem.AABB> blockBoxes, int2 terrainSize,
            TerrainSystem.AABB box)
        {
            // check nearby boxes -- TODO bias by half a box
            int xMin = (int)math.floor((box.Min.x + 0.5f) / TerrainSystem.BOX_SPACING);
            int xMax = (int)math.floor((box.Max.x + 0.5f) / TerrainSystem.BOX_SPACING);
            int zMin = (int)math.floor((box.Min.z + 0.5f) / TerrainSystem.BOX_SPACING);
            int zMax = (int)math.floor((box.Max.z + 0.5f) / TerrainSystem.BOX_SPACING);

            xMin = math.max(0, xMin);
            xMax = math.min(terrainSize.x - 1, xMax);
            zMin = math.max(0, zMin);
            zMax = math.min(terrainSize.y - 1, zMax);

            for (int z = zMin; z <= zMax; z++)
            {
                for (int x = xMin; x <= xMax; x++)
                {
                    int blockIndex = TerrainSystem.BlockCoordsToIndex(new int2(x, z), terrainSize);
                    TerrainSystem.AABB blockBox = blockBoxes[blockIndex];
                    bool intersects = !(box.Max.x < blockBox.Min.x || box.Min.x > blockBox.Max.x ||
                                        box.Max.y < blockBox.Min.y || box.Min.y > blockBox.Max.y ||
                                        box.Max.z < blockBox.Min.z || box.Min.z > blockBox.Max.z);
                    if (intersects)
                    {
                        return true;
                    }
                }
            }

            // TODO: check tanks
            return false;
        }


        private static bool PathHitsTerrain(NativeArray<TerrainSystem.AABB> blockBoxes, int2 terrainSize, float3 start,
            float3 end, float paraA, float paraB, float paraC)
        {
            float distance = math.distance(end.xz, start.xz);
            int steps = math.max(2, (int)math.ceil(distance / TerrainSystem.BOX_SPACING) + 1);
            steps = (int)math.ceil(steps * COLLISION_STEP_MULTIPLIER);

            for (int i = 0; i < steps; i++)
            {
                float t = i / (steps - 1f);
                float3 pos = GetSimulatedPosition(start, end, paraA, paraB, paraC, t);
                TerrainSystem.AABB box = new TerrainSystem.AABB
                {
                    Min = pos - CANNONBALL_RADIUS,
                    Max = pos + CANNONBALL_RADIUS,
                };
                if (BoxHitsTerrain(blockBoxes, terrainSize, box))
                {
                    return true;
                }
            }

            return false;
        }

        struct FireCannonballJob : IJobForEachWithEntity<LocalToWorld, TankFireCountdown, Rotation>
        {
            public float3 PlayerPosition;
            [ReadOnly]
            public NativeArray<float> BlockHeights;
            [ReadOnly]
            public NativeArray<TerrainSystem.AABB> BlockBoundingBoxes;
            
            public int2 TerrainSize;
            public float FirePeriod;
            public float DeltaTime;
            public float ElapsedTime;
            public EntityCommandBuffer.Concurrent CommandBuffer;
            public Entity CannonballPrefab;

            public void Execute(Entity _, int index, [ReadOnly] ref LocalToWorld localToWorld, ref TankFireCountdown countdown,
                ref Rotation rotation)
            {
                // launching cannonball
                countdown.SecondsUntilFire -= DeltaTime;
                if (countdown.SecondsUntilFire <= 0)
                {
                    countdown.SecondsUntilFire = math.fmod(countdown.SecondsUntilFire, FirePeriod) + FirePeriod;
                    // start and end positions
                    int2 playerBoxCoords = TerrainSystem.PositionToBlockCoords(PlayerPosition.x, PlayerPosition.z, TerrainSize);
                    int playerBoxIndex = TerrainSystem.BlockCoordsToIndex(playerBoxCoords, TerrainSize);
                    float playerBoxHeight = BlockHeights[playerBoxIndex];
                    float3 startPos = math.mul(localToWorld.Value, new float4(0,0,0,1)).xyz;
                    float3 endPos = TerrainSystem.BlockCoordsToPosition(playerBoxCoords, playerBoxHeight, TerrainSize);
                    endPos.y += CANNONBALL_RADIUS + 0.01f; // extra 0.01 to avoid false positive collisions at endPos
                    float distance = math.distance(endPos.xz, startPos.xz);
                    float duration = distance / CANNONBALL_SPEED;
                    if (duration < 0.0001f)
                    {
                        duration = 1.0f;
                    }
    
                    // binary searching to determine height of cannonball arc
                    float low = math.max(startPos.y, endPos.y);
                    float high = low * 2;
                    float paraA, paraB, paraC;
    
                    // look for height of arc that won't hit boxes
                    while (true)
                    {
                        Parabola.Create(startPos.y, high, endPos.y, out paraA, out paraB, out paraC);
                        if (!PathHitsTerrain(BlockBoundingBoxes, TerrainSize, startPos, endPos, paraA, paraB, paraC))
                        {
                            // high enough
                            break;
                        }
    
                        // not high enough.  Double value
                        low = high;
                        high *= 2;
                        // failsafe
                        if (high > 9999)
                        {
                            return; // skip launch
                        }
                    }
                    // do binary searches to narrow down
                    while (high - low > CANNONBALL_PARABOLA_PRECISION)
                    {
                        float mid = (low + high) / 2;
                        Parabola.Create(startPos.y, mid, endPos.y, out paraA, out paraB, out paraC);
                        if (PathHitsTerrain(BlockBoundingBoxes, TerrainSize, startPos, endPos, paraA, paraB, paraC))
                        {
                            // not high enough
                            low = mid;
                        }
                        else
                        {
                            // too high
                            high = mid;
                        }
                    }
    
                    // launch with calculated height
                    float height = (low + high) / 2;
                    Parabola.Create(startPos.y, height, endPos.y, out paraA, out paraB, out paraC);

                    Entity ballEntity = CommandBuffer.Instantiate(index, CannonballPrefab);
                    CommandBuffer.SetComponent(index, ballEntity, new Translation {Value = startPos});
                    CommandBuffer.SetComponent(index, ballEntity, new ArcState
                    {
                        Parabola = new float3(paraA, paraB, paraC),
                        StartTime = ElapsedTime,
                        Duration = duration,
                        StartPos = startPos,
                        EndPos = endPos,
                    });

                    // set cannon rotation
                    {
                        const float t = 0.01f;
                        float altitudeAngle = -math.atan2(
                            Parabola.Solve(paraA, paraB, paraC, t) - Parabola.Solve(paraA, paraB, paraC, 0.0f),
                            t * distance);
                        rotation.Value = quaternion.Euler(altitudeAngle, 0, 0);
                    }
                }
            }
        }

        private Entity _cannonballPrefabEntity;
        public void CreateCannonballPrefab()
        {
            EntityArchetype cannonballPrefabArchetype = EntityManager.CreateArchetype(
                typeof(Translation), typeof(Scale), typeof(LocalToWorld), typeof(ArcState), typeof(RenderMesh), typeof(Prefab));
            _cannonballPrefabEntity = EntityManager.CreateEntity(cannonballPrefabArchetype);
            EntityManager.SetComponentData(_cannonballPrefabEntity,
                new Scale {Value = CANNONBALL_RADIUS});
            if (_options == null)
            {
                _options = GameObject.Find("Options").GetComponent<Options>();
            }
            EntityManager.SetSharedComponentData(_cannonballPrefabEntity, _options.bulletLook.Value);            
        }
        
        private Options _options;
        private TerrainSystem _terrain;
        private PlayerPositionCacheSystem _playerPosCache;
        private BeginSimulationEntityCommandBufferSystem _barrier;
        protected override void OnCreateManager()
        {
            _terrain = World.GetOrCreateSystem<TerrainSystem>();
            _playerPosCache = World.GetOrCreateSystem<PlayerPositionCacheSystem>();
            _barrier = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            if (_options == null)
            {
                _options = GameObject.Find("Options").GetComponent<Options>();
            }

            inputDeps = JobHandle.CombineDependencies(inputDeps, _terrain.UpdateCacheJob);
            JobHandle outputJob = new FireCannonballJob
            {
                PlayerPosition = _playerPosCache.PlayerPosition,
                BlockHeights = _terrain.CachedBlockHeights,
                BlockBoundingBoxes = _terrain.CachedBlockBoundingBoxes,
                TerrainSize = _terrain.TerrainSize,
                FirePeriod = _options.tankLaunchPeriod,
                DeltaTime = Time.deltaTime,
                ElapsedTime = Time.time,
                CommandBuffer = _barrier.CreateCommandBuffer().ToConcurrent(),
                CannonballPrefab = _cannonballPrefabEntity,
            }.Schedule(this, inputDeps);
            _barrier.AddJobHandleForProducer(outputJob);
            return outputJob;
        }
    }
}