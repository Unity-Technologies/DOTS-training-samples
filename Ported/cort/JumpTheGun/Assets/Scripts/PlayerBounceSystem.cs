using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace JumpTheGun
{
    [UpdateAfter(typeof(TerrainSystem))]
    public class PlayerBounceSystem : JobComponentSystem
    {
        private const float BOUNCE_HEIGHT = 2.0f;
        private const float BOUNCE_BASE_DURATION = 0.7f;

        [BurstCompile]
        struct UpdateBounceJob : IJobForEach<Translation, ArcState>
        {
            public float ElapsedTime;
            public int2 MouseBoxCoords;
            public int2 TerrainSize;
            [ReadOnly]
            public NativeArray<float> BlockHeights;
            [ReadOnly]
            public NativeArray<byte> IsBlockOccupied;

            public void Execute(ref Translation position, ref ArcState arc)
            {
                float t = ElapsedTime - arc.StartTime;
                if (t > arc.Duration)
                {
                    // Compute the next arc info
                    int2 playerCoords = TerrainSystem.PositionToBlockCoords(position.Value.x, position.Value.z, TerrainSize);
                    int2 dir = math.clamp(MouseBoxCoords - playerCoords,
                        new int2(-1,-1), new int2(1,1));
                    int2 destCoords = playerCoords + dir;
                    // If destination is outside the terrain bounds, don't move.
                    if (destCoords.x < 0 || destCoords.x >= TerrainSize.x ||
                        destCoords.y < 0 || destCoords.y >= TerrainSize.y)
                    {
                        destCoords = playerCoords;
                    }
                    int playerBlockIndex = TerrainSystem.BlockCoordsToIndex(playerCoords, TerrainSize);
                    int destBlockIndex = TerrainSystem.BlockCoordsToIndex(destCoords, TerrainSize);
                    // If destination is occupied, don't move.
                    if (IsBlockOccupied[destBlockIndex] != 0)
                    {
                        destCoords = playerCoords;
                        destBlockIndex = playerBlockIndex;
                    }

                    float playerBlockHeight = BlockHeights[playerBlockIndex];
                    float destBlockHeight = BlockHeights[destBlockIndex];
                    float parabolaHeight = math.max(playerBlockHeight, destBlockHeight);
                    if (dir.x != 0 && dir.y != 0)
                    {
                        // If jumping diagonally, the parabola height must take into account the directly
                        // adjacent blocks as well.
                        int2 adjacentCoordsX = playerCoords + new int2(dir.x, 0);
                        if (adjacentCoordsX.x >= 0 && adjacentCoordsX.x < TerrainSize.x)
                        {
                            int adjacentIndexX = TerrainSystem.BlockCoordsToIndex(adjacentCoordsX, TerrainSize);
                            float adjacentHeightX = BlockHeights[adjacentIndexX];
                            parabolaHeight = math.max(parabolaHeight, adjacentHeightX);
                        }
                        int2 adjacentCoordsZ = playerCoords + new int2(0, dir.y);
                        if (adjacentCoordsZ.y >= 0 && adjacentCoordsZ.y < TerrainSize.y)
                        {
                            int adjacentIndexZ = TerrainSystem.BlockCoordsToIndex(adjacentCoordsZ, TerrainSize);
                            float adjacentHeightZ = BlockHeights[adjacentIndexZ];
                            parabolaHeight = math.max(parabolaHeight, adjacentHeightZ);
                        }
                    }
                    parabolaHeight += BOUNCE_HEIGHT;

                    Parabola.Create(playerBlockHeight, parabolaHeight, destBlockHeight,
                        out arc.Parabola.x, out arc.Parabola.y, out arc.Parabola.z);
                    arc.StartPos =
                        TerrainSystem.BlockCoordsToPosition(playerCoords, playerBlockHeight, TerrainSize);
                    arc.EndPos =
                        TerrainSystem.BlockCoordsToPosition(destCoords, destBlockHeight, TerrainSize);
                    arc.StartTime = ElapsedTime;
                    float distance = math.distance(playerCoords, destCoords);
                    arc.Duration = math.max(1, distance) * BOUNCE_BASE_DURATION;
                }
                else
                {
                    // Get the current height along the parabola
                    float s = t / arc.Duration;
                    position.Value = new float3(
                        math.lerp(arc.StartPos.x, arc.EndPos.x, s),
                        Parabola.Solve(arc.Parabola.x, arc.Parabola.y, arc.Parabola.z, s),
                        math.lerp(arc.StartPos.z, arc.EndPos.z, s)
                    );
                }
            }
        }

        private EntityQuery _playerQuery;
        private Options _options;
        private TerrainSystem _terrain;
        private TankAimSystem _tankAimSystem;
        protected override void OnCreateManager()
        {
            _playerQuery = GetEntityQuery(new ComponentType[]
            {
                ComponentType.ReadWrite<Translation>(),
                ComponentType.ReadWrite<ArcState>(),
                ComponentType.Exclude<Scale>(),
            });
            _terrain = World.GetOrCreateSystem<TerrainSystem>();
            _tankAimSystem = World.GetOrCreateSystem<TankAimSystem>();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            if (_options == null)
            {
                _options = GameObject.Find("Options").GetComponent<Options>();
            }
            // Determine which block is under the mouse cursor.
            float meanBlockHeight = (_options.terrainHeightMin + _options.terrainHeightMax) / 2;
            Vector3 mouseWorldPos = new Vector3(0, meanBlockHeight, 0);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float t = (meanBlockHeight - ray.origin.y) / ray.direction.y;
            mouseWorldPos.x = ray.origin.x + t * ray.direction.x;
            mouseWorldPos.z = ray.origin.z + t * ray.direction.z;
            inputDeps = JobHandle.CombineDependencies(inputDeps, _terrain.UpdateCacheJob);
            int2 mouseBoxCoords = TerrainSystem.PositionToBlockCoords(mouseWorldPos.x, mouseWorldPos.z, _terrain.TerrainSize);

            return new UpdateBounceJob
            {
                ElapsedTime = Time.time,
                MouseBoxCoords = mouseBoxCoords,
                TerrainSize = new int2(_options.terrainSizeX, _options.terrainSizeZ),
                BlockHeights = _terrain.CachedBlockHeights,
                IsBlockOccupied = _terrain.CachedIsBlockOccupied,

            }.Schedule(_playerQuery, inputDeps);
        }
    }
}
