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

            var currCellWallFlags = (WallFlags)grid[MazeUtils.CellIdxFromPos(currCell, gameConfig.mazeSize)].wallFlags;
            var nextCellWallFlags = WallFlags.All;
            if(nextCell.x >= 0 && nextCell.x < gameConfig.mazeSize && nextCell.y >= 0 && nextCell.y < gameConfig.mazeSize)
                nextCellWallFlags = (WallFlags)grid[MazeUtils.CellIdxFromPos(nextCell, gameConfig.mazeSize)].wallFlags;
            
            MazeUtils.DrawGridCell(currCell, (byte)currCellWallFlags);
            
            // todo: perform collision block in outside func
            // **COLLISION**
            // going left
            if (cellDiff.x < 0)
            {
                var posPush = CellPushNS(playerPosition, playerRadius);
                if (MazeUtils.HasFlag(currCellWallFlags, WallFlags.West) 
                    || MazeUtils.HasFlag(nextCellWallFlags, WallFlags.East))
                {
                    movementDelta.x = 0;
                }
                // if no west wall and walking into edge, add push
                else if (posPush.z > 0 && MazeUtils.HasFlag(nextCellWallFlags, WallFlags.South) ||
                         posPush.z < 0 && MazeUtils.HasFlag(nextCellWallFlags, WallFlags.North))
                {
                    movementDelta.z += posPush.z;
                }
            }
            // going right
            if (cellDiff.x > 0)
            {
                var posPush = CellPushNS(playerPosition, playerRadius);
                if (MazeUtils.HasFlag(currCellWallFlags, WallFlags.East)
                     || MazeUtils.HasFlag(nextCellWallFlags, WallFlags.West))
                {
                    movementDelta.x = 0;
                }
                // if no east wall and walking into edge, add push
                else if (posPush.z > 0 && MazeUtils.HasFlag(nextCellWallFlags, WallFlags.South) ||
                         posPush.z < 0 && MazeUtils.HasFlag(nextCellWallFlags, WallFlags.North))
                {
                    movementDelta.z += posPush.z;
                }
            }
            // going north
            if (cellDiff.y > 0)
            {
                var posPush = CellPushEW(playerPosition, playerRadius);
                if (MazeUtils.HasFlag(currCellWallFlags, WallFlags.North)
                     || MazeUtils.HasFlag(nextCellWallFlags, WallFlags.South))
                {
                    movementDelta.z = 0;
                }
                // if no south wall and walking into edge, add push
                else if (posPush.x > 0 && MazeUtils.HasFlag(nextCellWallFlags, WallFlags.West) ||
                         posPush.x < 0 && MazeUtils.HasFlag(nextCellWallFlags, WallFlags.East))
                {
                    movementDelta.x += posPush.x;
                }
            }
            // going south
            if (cellDiff.y < 0)
            {
                var posPush = CellPushEW(playerPosition, playerRadius);
                if (MazeUtils.HasFlag(currCellWallFlags, WallFlags.South)
                     || MazeUtils.HasFlag(nextCellWallFlags, WallFlags.North))
                {
                    movementDelta.z = 0;
                }
                // if no north wall and walking into edge, add push
                else if (posPush.x > 0 && MazeUtils.HasFlag(nextCellWallFlags, WallFlags.West) ||
                         posPush.x < 0 && MazeUtils.HasFlag(nextCellWallFlags, WallFlags.East))
                {
                    movementDelta.x += posPush.x;
                }
            }
            // **END COLLISION**

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
    
    private float3 CellPushEW(float3 pos, float radius)
    {
        return CalcCellPush(pos, radius, math.right(), math.left());
    }

    private float3 CellPushNS(float3 pos, float radius)
    {
        return CalcCellPush(pos, radius, math.forward(), math.back());
    }

    private float3 CalcCellPush(float3 pos, float radius, float3 fwdCellDir, float3 backCellDir)
    {
        var gridPosBack = MazeUtils.WorldPositionToGrid(pos + backCellDir);
        var gridPosFwd =  MazeUtils.WorldPositionToGrid(pos + fwdCellDir);
        var backWorldPos = MazeUtils.GridPositionToWorld(gridPosBack.x, gridPosBack.y);
        var fwdWorldPos =  MazeUtils.GridPositionToWorld(gridPosFwd.x, gridPosFwd.y);
        var backDist = math.distancesq(backWorldPos, pos);
        var fwdDist = math.distancesq(fwdWorldPos, pos);

        var radiusPad = 1.7f; // check around middle 70% of character
        var radiusPush = radius * (2 - radiusPad);
        var paddedRadiusSquare = math.pow(radius * radiusPad, 2); 
        
        if (backDist < fwdDist && backDist < paddedRadiusSquare)
            return fwdCellDir * radiusPush;
        
        if (fwdDist < backDist && fwdDist < paddedRadiusSquare)
            return backCellDir * radius * 0.3f;

        return float3.zero;
    }
}