using ECSExamples;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class WalkSystem : JobComponentSystem
{
	private EntityQuery m_BoardQuery;
	private EntityQuery m_ConfigQuery;
	private EndSimulationEntityCommandBufferSystem m_Buffer;
	private BoardSystem m_Board;

	protected override void OnCreate()
	{
		m_BoardQuery = GetEntityQuery(typeof(BoardDataComponent));
		m_ConfigQuery = GetEntityQuery(typeof(GameConfigComponent));
		m_Buffer = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
		m_Board = World.GetExistingSystem<BoardSystem>();
	}

	protected override JobHandle OnUpdate(JobHandle inputDep)
	{
		var gameConfig = m_ConfigQuery.GetSingleton<GameConfigComponent>();
		var board = m_BoardQuery.GetSingleton<BoardDataComponent>();
		var cellMap = m_Board.CellMap;
		var arrowMap = m_Board.ArrowMap;
		var ecb = m_Buffer.CreateCommandBuffer().ToConcurrent();
		var deltaTime = Time.deltaTime;
		var job = Entities.ForEach((Entity entity, int entityInQueryIndex , ref WalkComponent walker, ref Translation position, ref Rotation rotation) =>
		//Entities.ForEach((Entity entity, ref WalkComponent walker, ref Translation position, ref Rotation rotation) =>
		{
			float3 fwd = ForwardVectorFromRotation(rotation.Value);

			//if (!board || state == State.Dead)
			//return;

			// cap movement if the FPS is low to one cell
			float remainingDistance = walker.Speed * deltaTime;

			// return to whole numbers
			var newPos = position.Value;
			{
				float speed = deltaTime * 10f;
				if (Mathf.Abs(fwd.x) > 0)
					newPos.z = Mathf.Lerp(newPos.z, Mathf.Round(newPos.z), speed);
				else if (Mathf.Abs(fwd.z) > 0)
					newPos.x = Mathf.Lerp(newPos.x, Mathf.Round(newPos.x), speed);
				position.Value = newPos;
			}

			// Apply walk deltas in a loop so that even if we have a super low framerate, we
			// don't skip cells in the board.
			while (true)
			{
				float slice = Mathf.Min(board.cellSize.x * 0.3f, remainingDistance);
				remainingDistance -= slice;
				if (slice <= 0f || slice < math.FLT_MIN_NORMAL*8f)
				{
					//Debug.Log("No change");
					break;
				}

				var delta = fwd * slice;
				newPos.x += delta.x;
				newPos.y += delta.y;
				newPos.z += delta.z;
				//Debug.Log("Delta " + delta + " fwd=" + fwd + " slice=" + slice);
				//var myDirection = Walks.DirectionFromVector(fwd);
				var myDirection = DirectionFromRotation(rotation.Value);

				// TODO: seems this always resets position to ~0,0,0
				// Round position values for checking the board. This is so that
				// we collide with arrows and walls at the right time.
				position.Value = newPos;
				if (myDirection == Direction.North)
					newPos.z = Mathf.Floor(newPos.z);
				else if (myDirection == Direction.South)
					newPos.z = Mathf.Ceil(newPos.z);
				else if (myDirection == Direction.East)
					newPos.x = Mathf.Floor(newPos.x);
				else if (myDirection == Direction.West)
					newPos.x = Mathf.Ceil(newPos.x);
				else
					throw new System.ArgumentOutOfRangeException();

				//var vect = new Vector3 {x = position.Value.x, y = position.Value.y, z = position.Value.z};
				//var cell = m_Board.CellAtWorldPosition(vect);

				var localPt = new float2(newPos.x, newPos.z);
				localPt += board.cellSize * 0.5f; // offset by half cellsize
				var cellCoord = new float2(Mathf.FloorToInt(localPt.x / board.cellSize.x),
					Mathf.FloorToInt(localPt.y / board.cellSize.y));
				var cellIndex = (int) (cellCoord.y * board.size.x + cellCoord.x);

				if (cellIndex >= board.size.x * board.size.y)
				{
					ecb.DestroyEntity(nativeThreadIndex, entity);
					//PostUpdateCommands.DestroyEntity(entity);
					return;
				}

				//Debug.Log("On idx=" + cellIndex + " coord=" + cellCoord.x + "," + cellCoord.y + " pos=" + position.Value.x + "," + position.Value.y);
				CellComponent cell = new CellComponent();

				var newDirection = myDirection;
				// Nothing on this cell, no change to direction
				if (!cellMap.TryGetValue(cellIndex, out cell))
				{
					newDirection = myDirection;
					return;
				}

				if ((cell.data & CellData.Hole) == CellData.Hole)
				{
					ecb.DestroyEntity(nativeThreadIndex, entity);
					return;
				}

				ArrowComponent arrow;
				arrowMap.TryGetValue(cellIndex, out arrow);
				newDirection = ShouldRedirect(cell, cellIndex, myDirection, arrow.Direction,
					gameConfig.DiminishesArrows);

				//newDirection = cell.ShouldRedirect(myDirection, gameConfig.DiminishesArrows);

				//Debug.Log("Forward=" + fwd + " oldDir=" + myDirection + " newDir="+ newDirection);
				if (newDirection != myDirection)
				{
					rotation.Value = RotationForDirection(newDirection);
					if (myDirection == Cell.OppositeDirection(newDirection))
					{
						// Turn around fast when it's the opposite direction.
						//transform.forward = Forward;
					}
				}
			}

			// TODO: Switch state logic
			/*if (state == State.Falling)
			{
				transform.position += Vector3.down * fallingSpeed * Time.deltaTime;
				if (transform.position.y < DIE_AT_DEPTH)
					Destroy(gameObject);
			}*/

			// TODO: set transform.forward
			// Lerp the visible forward direction towards the logical one each frame.
			//return new Vector3(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t, a.z + (b.z - a.z) * t);
			//transform.forward = Vector3.Lerp(transform.forward, Forward, Time.deltaTime * RotationSpeed);
			/*var fwd = walker.Forward;
			var t = Time.deltaTime * gameConfig.RotationSpeed;
			newPos.x = newPos.x + (fwd.x - newPos.x) * t;
			newPos.y = newPos.y + (fwd.y - newPos.y) * t;
			newPos.z = newPos.z + (fwd.z - newPos.z) * t;*/
		}).WithReadOnly(cellMap).WithReadOnly(arrowMap).Schedule(inputDep);
		return job;
	}

	int PositionToIndex(BoardDataComponent board, Translation position)
	{
		var localPt = new float2(position.Value.x, position.Value.z);
		localPt += board.cellSize * 0.5f; // offset by half cellsize
		var cellCoord = new float2(Mathf.FloorToInt(localPt.x / board.cellSize.x), Mathf.FloorToInt(localPt.y / board.cellSize.y));
		return (int)(cellCoord.y * board.size.x + cellCoord.x);
	}

	static quaternion RotationForDirection(Direction dir) {
		switch (dir) {
			case Direction.North:
				return quaternion.identity;
			case Direction.South:
				return quaternion.RotateY(math.PI);
			case Direction.East:
				return quaternion.RotateY(math.PI/2);
			case Direction.West:
				return quaternion.RotateY(3*math.PI/2);
			default:
				throw new System.ArgumentOutOfRangeException();
		}
	}

	public quaternion RotationForDirection2(Direction dir) {
		switch (dir) {
			case Direction.North:
				return quaternion.identity;
			case Direction.South:
				return quaternion.AxisAngle(new float3(0,0,1), math.PI/2);
			case Direction.East:
				return quaternion.AxisAngle(new float3(0,0,1), math.PI/4);
			case Direction.West:
				return quaternion.AxisAngle(new float3(0,0,1), 3*math.PI/4);
			default:
				throw new System.ArgumentOutOfRangeException(dir.ToString());
		}
	}

	static Vector3 ForwardVectorFromRotation(quaternion rotation)
	{
		var dot = math.dot(rotation, quaternion.RotateY(math.PI));
		if (math.abs(dot) > 0.9f)
			return Vector3.back;
		dot = math.dot(rotation, quaternion.RotateY(math.PI / 2));
		if (math.abs(dot) > 0.9f)
			return Vector3.right;
		dot = math.dot(rotation, quaternion.RotateY(3*math.PI / 2));
		if (math.abs(dot) > 0.9f)
			return Vector3.left;
		return Vector3.forward;
	}

	static Direction DirectionFromRotation(quaternion rotation)
	{
		var dot = math.dot(rotation, quaternion.RotateY(math.PI));
		if (math.abs(dot) > 0.9f)
			return Direction.South;
		dot = math.dot(rotation, quaternion.RotateY(math.PI / 2));
		if (math.abs(dot) > 0.9f)
			return Direction.East;
		dot = math.dot(rotation, quaternion.RotateY(3*math.PI / 2));
		if (math.abs(dot) > 0.9f)
			return Direction.West;
		return Direction.North;
	}

	CellData CellDirectionFromRotation(quaternion rotation)
	{
		var dot = math.dot(rotation, quaternion.RotateY(math.PI));
		if (math.abs(dot) > 0.9f)
			return CellData.WallSouth;
		dot = math.dot(rotation, quaternion.RotateY(math.PI / 2));
		if (math.abs(dot) > 0.9f)
			return CellData.WallEast;
		dot = math.dot(rotation, quaternion.RotateY(3*math.PI / 2));
		if (math.abs(dot) > 0.9f)
			return CellData.WallWest;
		return CellData.WallNorth;
	}

	static Direction ShouldRedirect(CellComponent cell, int cellIndex, Direction myDirection, Direction arrowDirection, bool diminishesArrows) {
		/*if (blockState == BlockState.Confuse) {
			const int numDirections = 4;
			var nextIndex = ((int)myDirection + 1 + Random.Range(0, numDirections - 1)) % numDirections;
			myDirection = (Direction)nextIndex;
		}*/

		if ((cell.data & CellData.Arrow) == CellData.Arrow)
		{
			if (myDirection != arrowDirection)
			{
				//if (arrowDirection == OppositeDirection(myDirection) && diminishesArrows)
				//	DiminishArrow();
				myDirection = arrowDirection;
			}
		}

		if (HasWall(cell, myDirection))
		{
			myDirection = Cell.DirectionWhenHitWall(myDirection);
			if (HasWall(cell, myDirection))
			{
				myDirection = Cell.OppositeDirection(myDirection);
				if (HasWall(cell, myDirection))
					myDirection = Cell.DirectionWhenHitWall(myDirection, 3);
			}
		}
		return myDirection;
	}

	static bool HasWall(CellComponent cell, Direction wall)
	{
		var cellDataWall = DirectionToCellData(wall);
		if ((cell.data & cellDataWall) == cellDataWall)
			return true;
		return false;
	}

	Direction CellDataWallToDirection(CellData cellData)
	{
		if ((cellData & CellData.WallSouth) == CellData.WallSouth)
			return Direction.South;
		if ((cellData & CellData.WallEast) == CellData.WallEast)
			return Direction.East;
		if ((cellData & CellData.WallWest) == CellData.WallWest)
			return Direction.West;
		return Direction.North;
	}

	static CellData DirectionToCellData(Direction dir)
	{
		if (dir == Direction.South)
			return CellData.WallSouth;
		if (dir == Direction.East)
			return CellData.WallWest;
		if (dir == Direction.West)
			return CellData.WallEast;
		return CellData.WallNorth;
	}
}