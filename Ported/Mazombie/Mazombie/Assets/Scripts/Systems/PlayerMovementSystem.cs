using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(SimulationSystemGroup))]
[BurstCompile]
public partial struct PlayerMovementSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Player>();
        state.RequireForUpdate<GridCell>();
    }
    
    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }
    
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var gameConfig = SystemAPI.GetSingleton<GameConfig>();
        var gameConfigEntity = SystemAPI.GetSingletonEntity<GameConfig>();
        var playerInput = SystemAPI.GetSingleton<PlayerInput>();
        var activeCamEntity = SystemAPI.GetSingletonEntity<ActiveCamera>();
        var grid = SystemAPI.GetBuffer<GridCell>(gameConfigEntity);

        var ltwLookup = SystemAPI.GetComponentLookup<LocalToWorldTransform>();
        var cameraLTW = ltwLookup[activeCamEntity];
        
        float3 cameraFwd = math.mul(cameraLTW.Value.Rotation, math.forward());
        float3 cameraRight = math.mul(cameraLTW.Value.Rotation, math.right());
        float3 cameraForwardOnUpPlane = math.normalizesafe(cameraFwd - math.projectsafe(cameraFwd, math.up()));

        float3 movementDir = playerInput.movement.y * cameraForwardOnUpPlane + playerInput.movement.x * cameraRight;
        movementDir = math.normalizesafe(movementDir);
        
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        foreach (var playerAspect in SystemAPI.Query<PlayerAspect>())
        {
            float playerRadius = 0.45f; // TODO: get from mesh half-size?
            float3 playerPosition = playerAspect.Transform.ValueRW.Value.Position;
            
            float3 movementDelta = movementDir
                                   * playerAspect.Player.ValueRO.speed
                                   * SystemAPI.Time.DeltaTime;
            
            int2 currCell = MazeUtils.WorldPositionToGrid(playerPosition);
            int2 nextCell = MazeUtils.WorldPositionToGrid(
                playerPosition
                + movementDelta
                + playerRadius * movementDir
                );
            int2 cellDiff = nextCell - currCell;

            var currCellWallFlags = grid[MazeUtils.CellIdxFromPos(currCell, gameConfig.mazeSize)].wallFlags;
            var nextCellWallFlags = (byte)WallFlags.All;
            if(nextCell.x >= 0 && nextCell.x < gameConfig.mazeSize && nextCell.y >= 0 && nextCell.y < gameConfig.mazeSize)
                nextCellWallFlags = grid[MazeUtils.CellIdxFromPos(nextCell, gameConfig.mazeSize)].wallFlags;
            
            MazeUtils.DrawGridCell(currCell, currCellWallFlags);
            
            // going left
            if (cellDiff.x < 0)
            {
                if ((currCellWallFlags & (byte)WallFlags.West) != 0 
                    || (nextCellWallFlags & (byte)WallFlags.East) != 0)
                {
                    movementDelta.x = 0;
                }
            }
            // going right
            if (cellDiff.x > 0)
            {
                if ((currCellWallFlags & (byte)WallFlags.East) != 0
                    || (nextCellWallFlags & (byte)WallFlags.West) != 0)
                {
                    movementDelta.x = 0;
                }
            }
            // going north
            if (cellDiff.y > 0)
            {
                if ((currCellWallFlags & (byte)WallFlags.North) != 0
                || (nextCellWallFlags & (byte)WallFlags.South) != 0)
                {
                    movementDelta.z = 0;
                }
            }
            // going south
            if (cellDiff.y < 0)
            {
                if ((currCellWallFlags & (byte) WallFlags.South) != 0
                || (nextCellWallFlags & (byte) WallFlags.North) != 0)
                {
                    movementDelta.z = 0;
                }
            }

            // Move & Rotate player based on updated movement vector
            playerAspect.Transform.ValueRW.Value.Position += movementDelta;
            if (math.lengthsq(movementDelta) > 0)
            {
                playerAspect.Transform.ValueRW.Value.Rotation =
                    math.slerp(
                         playerAspect.Transform.ValueRW.Value.Rotation, 
                         quaternion.LookRotationSafe(movementDir, math.up()),
                         playerAspect.Player.ValueRO.speed * 2.0f * SystemAPI.Time.DeltaTime
                        );   
            }

        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}