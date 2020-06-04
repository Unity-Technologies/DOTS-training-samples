using Unity.Collections;
using Unity.Entities;
using Unity.Entities.UniversalDelegates;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

class MovementSystem : SystemBase
{
    const float k_SliceEpsilon = 0.00001f;

    struct ArrowData
    {
        public int2 CellCoord;
        public GridDirection Direction;
    }

    //NativeList<ArrowData> m_Arrows;
    //NativeArray<bool> m_CellContainsArrowGrid;

    EndSimulationEntityCommandBufferSystem m_Barrier;

    protected override void OnCreate()
    {
        m_Barrier = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var gridSystem = World.GetOrCreateSystem<GridCreationSystem>();
        if (!gridSystem.Cells.IsCreated)
            return;

        var cells = gridSystem.Cells;
        var rows = ConstantData.Instance.BoardDimensions.x;
        var cols = ConstantData.Instance.BoardDimensions.y;
        var cellSize = new float2(ConstantData.Instance.CellSize);

        var rotationSpeed = ConstantData.Instance.RotationSpeed;

        var deltaTime = Time.DeltaTime;

        var ecb = m_Barrier.CreateCommandBuffer().ToConcurrent();

        // find all arrows
        var arrows = new NativeList<ArrowData>(ConstantData.Instance.MaxArrows * ConstantData.Instance.NumPlayers, Allocator.TempJob);
        Entities
            .ForEach((in ArrowComponent arrow, in Direction2D dir) =>
            {
                arrows.Add(new ArrowData { CellCoord = arrow.GridCell, Direction = dir.Value });
            })
            .Schedule();

        // update walking
        Entities
            .WithNone<FallingTag>()
            .WithNone<ReachedBase>()
            .WithReadOnly(cells)
            .WithReadOnly(arrows)
            .ForEach((int entityInQueryIndex, Entity entity, ref Position2D pos, ref Rotation2D rot, ref Direction2D dir, in WalkSpeed speed) =>
            {
                // TODO low fps handling here

                float remainingDistance = speed.Value * deltaTime;

                int2 beforeCellCoord = GetGridCoordinateFromPositionAndDirection(pos.Value, dir.Value, cellSize, cols, rows);

                // Apply walk deltas in a loop so that even if we have a super low framerate, we
                // don't skip cells in the board.
                while (true)
                {
                    float slice = math.min(cellSize.x * 0.3f, remainingDistance);
                    remainingDistance -= slice;
                    if (slice <= 0f || Utility.NearlyEqual(0f, slice, k_SliceEpsilon))
                        break;

                    var delta = Utility.ForwardVectorForDirection(dir.Value) * slice;
                    pos.Value += delta;

                    int2 cellCoord = GetGridCoordinateFromPositionAndDirection(pos.Value, dir.Value, cellSize, cols, rows);

                    if (cellCoord.x != beforeCellCoord.x
                        || cellCoord.y != beforeCellCoord.y)
                    {
                        //Debug.Log($"cell is {cellCoord.x}, {cellCoord.y} travel {dir.Value.ToString()} pos {pos.Value}");

                        if (cellCoord.x < 0 || cellCoord.x >= cols || cellCoord.y < 0 || cellCoord.y >= rows)
                        {
                            ecb.AddComponent<FallingTag>(entityInQueryIndex, entity);
                            ecb.RemoveComponent<Position2D>(entityInQueryIndex, entity);
                            throw new System.ArgumentOutOfRangeException($"cell coordinates are out of range - {cellCoord.x}, {cellCoord.y}");
                        }

                        var cellIndex = (cellCoord.y * rows) + cellCoord.x;
                        var cell = cells[cellIndex];

                        //Debug.Log($"cell isHole {cell.IsHole()} - directions {(cell.CanTravel(GridDirection.NORTH) ? "N" : "")}{(cell.CanTravel(GridDirection.EAST) ? "E" : "")}{(cell.CanTravel(GridDirection.SOUTH) ? "S" : "")}{(cell.CanTravel(GridDirection.WEST) ? "W" : "")}");

                        if (cell.IsHole())
                        {
                            // add falling tag
                            ecb.AddComponent<FallingTag>(entityInQueryIndex, entity);
                            ecb.RemoveComponent<Position2D>(entityInQueryIndex, entity);
                            return;
                        }
                        else if (cell.IsBase())
                        {
                            // remove entity and score
                            ecb.AddComponent(entityInQueryIndex, entity, new ReachedBase { PlayerID = cell.GetBasePlayerId() });
                            return;
                        }
                        else
                        {
                            var newDirection = dir.Value;

                            // check for arrows
                            for (int i = 0; i < arrows.Length; i++)
                            {
                                var arrow = arrows[i];
                                if (arrow.CellCoord.x == cellCoord.x
                                    && arrow.CellCoord.y == cellCoord.y)
                                {
                                    newDirection = arrow.Direction;
                                }
                            }

                            if (!cell.CanTravel(newDirection))
                            {
                                //Debug.Log($"Can't travel in {newDirection.ToString()}");

                                var startingDirection = newDirection;

                                do
                                {
                                    byte byteDir = (byte)newDirection;
                                    byteDir *= 2;
                                    if (byteDir > (byte)GridDirection.WEST)
                                        byteDir = (byte)GridDirection.NORTH;
                                    newDirection = (GridDirection)byteDir;
                                }
                                while (!cell.CanTravel(newDirection)
                                        && newDirection != startingDirection);

                                //Debug.Log($"New direction is {newDirection.ToString()}");

                                if (newDirection == startingDirection)
                                    throw new System.InvalidOperationException("Unable to resolve cell travel. Is there a valid exit from this cell?");
                            }

                            if (dir.Value != newDirection)
                            {
                                dir.Value = newDirection;

                                // clamp position to the centre of the cell
                                pos.Value = new float2(cellSize.x * 0.5f + (cellCoord.x * cellSize.x), cellSize.y * 0.5f + (cellCoord.y * cellSize.y));
                            }
                        }
                    }

                    beforeCellCoord = cellCoord;

                    // Lerp the visible forward direction towards the logical one each frame.
                    var goalRot = Utility.DirectionToAngle(dir.Value);
                    rot.Value = math.lerp(rot.Value, goalRot, deltaTime * rotationSpeed);
                }
            })
            .WithName("UpdateWalking")
            //.WithoutBurst()
            .ScheduleParallel();

        // update falling
        var fallingSpeed = ConstantData.Instance.FallingSpeed;
        var fallingKillY = ConstantData.Instance.FallingKillY;

        Entities
            .WithAll<FallingTag>()
            .ForEach((int entityInQueryIndex, Entity entity, ref LocalToWorld ltw) =>
            {
                var pos = ltw.Position - new float3(0f, fallingSpeed * deltaTime, 0f);
                if (pos.y >= fallingKillY)
                    ltw.Value.c3 = new float4(pos.x, pos.y, pos.z, 1f);
                else
                    ecb.DestroyEntity(entityInQueryIndex, entity);
            })
            .WithName("UpdateFalling")
            .ScheduleParallel();

        m_Barrier.AddJobHandleForProducer(Dependency);

        // clean up memory
        arrows.Dispose(Dependency);
    }

    static int2 GetGridCoordinateFromPositionAndDirection(in float2 pos, GridDirection dir, in float2 cellSize, int cols, int rows)
    {
        var flooredPos = pos;
        flooredPos -= cellSize * 0.5f;
        flooredPos.x = math.clamp(flooredPos.x, 0f, cellSize.x * cols);
        flooredPos.y = math.clamp(flooredPos.y, 0f, cellSize.y * rows);

        // Round position values for checking the board. This is so that
        // we collide with arrows and walls at the right time.
        switch (dir)
        {
            case GridDirection.NORTH:
                flooredPos.y = Mathf.Floor(flooredPos.y);
                break;
            case GridDirection.SOUTH:
                flooredPos.y = Mathf.Ceil(flooredPos.y);
                break;
            case GridDirection.EAST:
                flooredPos.x = Mathf.Floor(flooredPos.x);
                break;
            case GridDirection.WEST:
                flooredPos.x = Mathf.Ceil(flooredPos.x);
                break;
        }

        return Utility.WorldPositionToGridCoordinates(flooredPos, cellSize);
    }
}
