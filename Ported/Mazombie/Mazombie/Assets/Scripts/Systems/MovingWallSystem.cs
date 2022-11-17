using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[UpdateInGroup(typeof(InitializationSystemGroup))]
[UpdateAfter(typeof(MazeGeneratorSystem))]
[BurstCompile]
public partial struct MovingWallSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<GameConfig>();
        state.RequireForUpdate<MovingWall>();
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
        var grid = SystemAPI.GetBuffer<GridCell>(gameConfigEntity);

        var ecb = new EntityCommandBuffer(Allocator.Temp);

        foreach (var movingWallAspect in SystemAPI.Query<MovingWallAspect>())
        {
            float3 movementDir = new float3(1, 0, 0);
            float3 movingWallPosition = movingWallAspect.Transform.ValueRW.Value.Position;
            float3 wallStartPosition = MazeUtils.GridPositionToWorld(movingWallAspect.GridPositions.ValueRO.gridStartX, movingWallAspect.GridPositions.ValueRO.gridStartY) - new float3(0,0,0.5f);
            float3 wallEndPosition = MazeUtils.GridPositionToWorld(movingWallAspect.GridPositions.ValueRO.gridEndX, movingWallAspect.GridPositions.ValueRO.gridEndY) - new float3(0,0,0.5f);
            float wallSpeed = movingWallAspect.Speed.ValueRW.speed;

             float3 movementDelta =
                 new float3( SystemAPI.Time.DeltaTime * wallSpeed, 0, 0);
            
             int2 currCell = MazeUtils.WorldPositionToGrid(movingWallPosition);
             int2 nextCell = MazeUtils.WorldPositionToGrid(
                 movingWallPosition
                 + movementDelta
                 + movingWallAspect.Transform.ValueRO.Value.Scale * movementDir
             );
            
            movingWallAspect.Transform.ValueRW.Value.Position += movementDelta;

            if (wallSpeed > 0 && (wallEndPosition.x - movingWallPosition.x) < 0)
            {
                movingWallAspect.Speed.ValueRW.speed = -1 * wallSpeed;
            }
            else if (wallSpeed < 0 && (movingWallPosition.x - wallStartPosition.x) < 0)
            {
                movingWallAspect.Speed.ValueRW.speed = -1 * wallSpeed;
            }
            
            var currCellWallFlags = grid[MazeUtils.CellIdxFromPos(currCell, gameConfig.mazeSize)].wallFlags;
            MazeUtils.DrawGridCell(currCell, currCellWallFlags);
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
        
    }
}
