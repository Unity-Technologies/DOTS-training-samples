using ECSExamples;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class WalkSystem : ComponentSystem
{
	private EntityQuery m_BoardQuery;
	private EntityQuery m_ConfigQuery;

	protected override void OnCreate()
	{
		m_BoardQuery = GetEntityQuery(typeof(BoardDataComponent));
		m_ConfigQuery = GetEntityQuery(typeof(GameConfigComponent));
	}

	//protected override JobHandle OnUpdate(JobHandle inputDep)
	protected override void OnUpdate()
	{
		var gameConfig = m_ConfigQuery.GetSingleton<GameConfigComponent>();
		var board = m_BoardQuery.GetSingleton<BoardDataComponent>();
		var cellMap = World.GetExistingSystem<BoardSystem>().CellMap;
		var homebaseMap = World.GetExistingSystem<BoardSystem>().HomeBaseMap;
		var catMap = World.GetExistingSystem<BoardSystem>().CatMap;

		//var job =
		//Entities.ForEach((Entity entity, in ref WalkComponent walker, ref Translation position, ref Rotation rotation) =>
		Entities.ForEach((Entity entity, ref WalkComponent walker, ref Translation position, ref Rotation rotation) =>
		{
			//float3 fwd = new float3(0, 0, 1);
			//float3 fwd = new float3(0, 0, walker.Speed);
			//fwd = math.mul(rotation.Value, fwd);
			float3 fwd = ForwardVectorFromRotation(rotation.Value);

			//if (!board || state == State.Dead)
				//return;

			//var cellSize = board.boardDesc.cellSize;

			//if (state == State.Walking)
			//{
				// cap movement if the FPS is low to one cell
				float remainingDistance = walker.Speed * Time.deltaTime;

				// return to whole numbers
				var newPos = position.Value;
				{
					float speed = Time.deltaTime * 10f;
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
					if (slice <= 0f || Mathf.Approximately(0f, slice))
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
					switch (myDirection)
					{
						case Direction.North:
							newPos.z = Mathf.Floor(newPos.z);
							break;
						case Direction.South:
							newPos.z = Mathf.Ceil(newPos.z);
							break;
						case Direction.East:
							newPos.x = Mathf.Floor(newPos.x);
							break;
						case Direction.West:
							newPos.x = Mathf.Ceil(newPos.x);
							break;
						default:
							throw new System.ArgumentOutOfRangeException(myDirection.ToString());
					}

					//var vect = new Vector3 {x = position.Value.x, y = position.Value.y, z = position.Value.z};
					//var cell = m_Board.CellAtWorldPosition(vect);

					var localPt = new float2(newPos.x, newPos.z);
					localPt += board.cellSize * 0.5f; // offset by half cellsize
					var cellCoord = new float2(Mathf.FloorToInt(localPt.x / board.cellSize.x), Mathf.FloorToInt(localPt.y / board.cellSize.y));
					var cellIndex = (int)(cellCoord.y * board.size.x + cellCoord.x);

					if (cellIndex >= board.size.x * board.size.y)
					{
						Debug.Log("Fell off the board");
						PostUpdateCommands.DestroyEntity(entity);
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
						Debug.Log("Fell into hole");
						return;
					}

					newDirection = ShouldRedirect(cell, myDirection, gameConfig.DiminishesArrows);

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

					// TODO: Switch state logic, move to new system
					/*if (cell == null)
					{
						state = State.Falling;
					}
					else
					{
						cell = board.CellAtWorldPosition(pos);
						if (!cell)
							continue;

						// TODO: Mouse specific logic
						if (gameObject.CompareTag("Mouse"))
							cell.LastWalkTime = Time.time;

						// TODO: More switch state logic
						if (cell.AbsorbsWalker(this))
						{
							Destroy(gameObject);
							state = State.Dead;
							break;
						}
					}*/
				}
			//}

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
		});
		//}).Schedule(inputDep);
		//return job;
	}

	int PositionToIndex(BoardDataComponent board, Translation position)
	{
		var localPt = new float2(position.Value.x, position.Value.z);
		localPt += board.cellSize * 0.5f; // offset by half cellsize
		var cellCoord = new float2(Mathf.FloorToInt(localPt.x / board.cellSize.x), Mathf.FloorToInt(localPt.y / board.cellSize.y));
		return (int)(cellCoord.y * board.size.x + cellCoord.x);
	}

	public quaternion RotationForDirection(Direction dir) {
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
				throw new System.ArgumentOutOfRangeException(dir.ToString());
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

	Vector3 ForwardVectorFromRotation(quaternion rotation)
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

	Direction DirectionFromRotation(quaternion rotation)
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

	public Direction ShouldRedirect(CellComponent cell, Direction myDirection, bool diminishesArrows) {
		/*if (blockState == BlockState.Confuse) {
			const int numDirections = 4;
			var nextIndex = ((int)myDirection + 1 + Random.Range(0, numDirections - 1)) % numDirections;
			myDirection = (Direction)nextIndex;
		}*/

		/*Direction arrowDirection;
		bool hasArrow = GetArrowDirection(out arrowDirection);
		if (hasArrow && myDirection != arrowDirection) {
			if (arrowDirection == OppositeDirection(myDirection) && diminishesArrows)
				DiminishArrow();

			myDirection = arrowDirection;
		}*/

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

		/*if (HasWallOrNeighborWall(cell, neighborCell, myDirection)) {
			myDirection = Cell.DirectionWhenHitWall(myDirection);
			if (HasWallOrNeighborWall(cell, neighborCell, myDirection)) {
				myDirection = Cell.OppositeDirection(myDirection);
				if (HasWallOrNeighborWall(cell, neighborCell, myDirection)) {
					myDirection = Cell.OppositeDirection(myDirection);
					myDirection = Cell.DirectionWhenHitWall(myDirection);
				}
			}
		}*/
		return myDirection;
	}

	bool HasWall(CellComponent cell, Direction wall)
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

	CellData DirectionToCellData(Direction dir)
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