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
            .WithReadOnly(arrows)
            .ForEach((int entityInQueryIndex, Entity entity, ref Position2D pos, ref Rotation2D rot, ref Direction2D dir, in WalkSpeed speed) =>
            {
                // TODO low fps handling here

                float remainingDistance = speed.Value * deltaTime;

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

                    var flooredPos = pos.Value;
                    flooredPos -= cellSize * 0.5f;
                    flooredPos.x = math.clamp(flooredPos.x, 0f, cellSize.x * cols);
                    flooredPos.y = math.clamp(flooredPos.y, 0f, cellSize.y * rows);

                    // Round position values for checking the board. This is so that
                    // we collide with arrows and walls at the right time.
                    switch (dir.Value)
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

                    int2 cellCoord = Utility.WorldPositionToGridCoordinates(flooredPos, cellSize);

                    //Debug.Log($"cell is {cellCoord.x}, {cellCoord.y} travel {dir.Value.ToString()} pos {pos.Value} floored {flooredPos}");

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
                    }
                    else if (cell.IsBase())
                    {
                        // remove entity and score
                        ecb.AddComponent(entityInQueryIndex, entity, new ReachedBase { PlayerID = cell.GetBasePlayerId() });
                    }
                    else
                    {
                        var newDirection = dir.Value;

                        // check for arrows
                        bool foundArrow = false;
                        for (int i = 0; i < arrows.Length; i++)
                        {
                            var arrow = arrows[i];
                            if (arrow.CellCoord.x == cellCoord.x
                                && arrow.CellCoord.y == cellCoord.y)
                            {
                                foundArrow = true;
                                newDirection = arrow.Direction;
                            }
                        }

                        if (!foundArrow)
                        {
                            if (!cell.CanTravel(newDirection))
                            {
                                //Debug.Log($"Can't travel in {newDirection.ToString()}");

                                do
                                {
                                    byte byteDir = (byte)newDirection;
                                    byteDir *= 2;
                                    if (byteDir > (byte)GridDirection.WEST)
                                        byteDir = (byte)GridDirection.NORTH;
                                    newDirection = (GridDirection)byteDir;
                                }
                                while (!cell.CanTravel(newDirection)
                                        && newDirection != dir.Value);

                                //Debug.Log($"New direction is {newDirection.ToString()}");

                                if (newDirection == dir.Value)
                                    throw new System.InvalidOperationException("Unable to resolve cell travel. Is there a valid exit from this cell?");
                            }
                        }

                        dir.Value = newDirection;
                    }

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
                var pos = ltw.Position - new float3(0f, 0f, fallingSpeed * deltaTime);
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
}



// OOTS version
/*
public class Walks : MonoBehaviour, ISpawnable {
    public Interval Speed = new Interval(0.9f, 1.2f);
    public float fallingSpeed = 2f;
    public Board board;
    public Vector3 Forward;
    public float RotationSpeed = 0.5f;

    public bool DiminishesArrows = false;

    float mySpeed;
    static float DIE_AT_DEPTH = -5.0f; // how far to fall before dying
    State state;

    enum State {
        Walking,
        Falling,
        Dead
    }

    void OnEnable() {
        Forward = transform.forward;
        mySpeed = Speed.RandomValue();
        state = State.Walking;
    }

    public void OnSpawned(Spawner spawner) {
        board = spawner.board;
        Forward = transform.forward = spawner.transform.forward;
    }

    static Direction DirectionFromVector(Vector3 forward) {
        if (forward == Vector3.forward)
            return Direction.North;
        else if (forward == -Vector3.forward)
            return Direction.South;
        else if (forward == Vector3.right)
            return Direction.East;
        else if (forward == -Vector3.right)
            return Direction.West;

        throw new System.ArgumentOutOfRangeException("invalid direction: " + forward);
    }

    static Vector3 ForwardVectorForDirection(Direction dir) {
        switch (dir) {
            case Direction.North:
                return Vector3.forward;
            case Direction.South:
                return -Vector3.forward;
            case Direction.East:
                return Vector3.right;
            case Direction.West:
                return -Vector3.right;
            default:
                throw new System.ArgumentOutOfRangeException(dir.ToString());
        }
    }
    
    static Vector3 ComponentVectorForDirection(Direction dir) {
        switch (dir) {
            case Direction.North:
            case Direction.South:
                return Vector3.forward;
            case Direction.East:
            case Direction.West:
                return Vector3.right;
            default:
                throw new System.ArgumentOutOfRangeException(dir.ToString());
        }
    }

    static Quaternion RotationForDirection(Direction direction) {
        return Quaternion.Euler(0, 90f * (float)direction, 0);
    }

    Vector2Int lastRedirectCoord = new Vector2Int(-1, -1);

    void Update () {
        if (!board || state == State.Dead)
            return;

        var cellSize = board.boardDesc.cellSize;

        if (state == State.Walking) {
            // cap movement if the FPS is low to one cell
            float remainingDistance = mySpeed * Time.deltaTime;

            // return to whole numbers
            {
                var p = transform.position;
                float speed = Time.deltaTime * 10f;
                if (Mathf.Abs(Forward.x) > 0) {
                    p.z = Mathf.Lerp(p.z, Mathf.Round(transform.position.z), speed);
                } else if (Mathf.Abs(Forward.z) > 0) {
                    p.x = Mathf.Lerp(p.x, Mathf.Round(transform.position.x), speed);
                }
                transform.position = p;
            }

            // Apply walk deltas in a loop so that even if we have a super low framerate, we
            // don't skip cells in the board.
            while (true) { 
                float slice = Mathf.Min(cellSize.x * 0.3f, remainingDistance);
                remainingDistance -= slice;
                if (slice <= 0f || Mathf.Approximately(0f, slice))
                    break;

                var delta = Forward * slice;
                transform.position += delta;
                var myDirection = DirectionFromVector(Forward);
                var pos = transform.position;

                // Round position values for checking the board. This is so that
                // we collide with arrows and walls at the right time.
                switch (myDirection) {
                    case Direction.North:
                        pos.z = Mathf.Floor(pos.z); break;
                    case Direction.South:
                        pos.z = Mathf.Ceil(pos.z); break;
                    case Direction.East:
                        pos.x = Mathf.Floor(pos.x); break;
                    case Direction.West:
                        pos.x = Mathf.Ceil(pos.x); break;
                    default:
                        throw new System.ArgumentOutOfRangeException(myDirection.ToString());
                }

                var cell = board ? board.CellAtWorldPosition(transform.position) : null;
                if (cell == null) {
                    state = State.Falling;
                } else {
                    cell = board.CellAtWorldPosition(pos);
                    if (!cell)
                        continue;

                    if (gameObject.CompareTag("Mouse"))
                        cell.LastWalkTime = Time.time;

                    if (cell.AbsorbsWalker(this)) {
                        Destroy(gameObject);
                        state = State.Dead;
                        break;
                    }

                    var newDirection = cell.ShouldRedirect(myDirection, ref lastRedirectCoord, this);;
                    if (newDirection != myDirection) {
                        Forward = ForwardVectorForDirection(newDirection);
                        if (myDirection == Cell.OppositeDirection(newDirection)) {
                            // Turn around fast when it's the opposite direction.
                            transform.forward = Forward;
                        }
                        myDirection = newDirection;
                    }
                }
            }
        }
        
        if (state == State.Falling) {
            transform.position += Vector3.down * fallingSpeed * Time.deltaTime;
            if (transform.position.y < DIE_AT_DEPTH)
                Destroy(gameObject);
        }

        // Lerp the visible forward direction towards the logical one each frame.
        transform.forward = Vector3.Lerp(transform.forward, Forward, Time.deltaTime * RotationSpeed);
    }

    void RotateAroundPoint(Vector3 centerPos, Direction myDirection, Quaternion newRotation) {
        var component = ComponentVectorForDirection(myDirection);
        var dist = Vector3.Distance(
            Vector3.Scale(transform.position, component),
            Vector3.Scale(centerPos, component));

        transform.position -= Forward * dist;
        transform.rotation = newRotation;
        transform.position += Forward * dist;
    } 
}
*/
