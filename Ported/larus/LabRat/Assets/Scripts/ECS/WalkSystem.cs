using ECSExamples;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(ServerSimulationSystemGroup))]
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
		var deltaTime = Time.DeltaTime;
		var job = Entities.ForEach((Entity entity, int entityInQueryIndex , ref WalkComponent walker, ref Translation position, ref Rotation rotation) =>
		{
			float3 fwd = ForwardVectorFromRotation(rotation.Value);

			// cap movement if the FPS is low to one cell
			float remainingDistance = walker.Speed * deltaTime;

			// return to whole numbers
			var newPos = position.Value;
			{
				float speed = deltaTime * 10f;
				if (math.abs(fwd.x) > 0)
					newPos.z = math.lerp(newPos.z, math.round(newPos.z), speed);
				else if (math.abs(fwd.z) > 0)
					newPos.x = math.lerp(newPos.x, math.round(newPos.x), speed);
				position.Value = newPos;
			}

			// Apply walk deltas in a loop so that even if we have a super low framerate, we
			// don't skip cells in the board.
			while (true)
			{
				float slice = math.min(board.cellSize.x * 0.3f, remainingDistance);
				remainingDistance -= slice;
				if (slice <= 0f || slice < math.FLT_MIN_NORMAL*8f)
					break;

				var delta = fwd * slice;
				newPos.x += delta.x;
				newPos.y += delta.y;
				newPos.z += delta.z;
				var myDirection = DirectionFromRotation(rotation.Value);

				// Round position values for checking the board. This is so that
				// we collide with arrows and walls at the right time.
				position.Value = newPos;
				if (myDirection == Direction.North)
					newPos.z = math.floor(newPos.z);
				else if (myDirection == Direction.South)
					newPos.z = math.floor(newPos.z);
				else if (myDirection == Direction.East)
					newPos.x = math.floor(newPos.x);
				else if (myDirection == Direction.West)
					newPos.x = math.floor(newPos.x);
				else
					throw new System.ArgumentOutOfRangeException();

				Util.PositionToCoordinates(position.Value, board, out var cellCoord, out var cellIndex);

				if (cellIndex >= board.size.x * board.size.y)
				{
					ecb.DestroyEntity(entityInQueryIndex, entity);
					return;
				}

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
					ecb.DestroyEntity(entityInQueryIndex, entity);
					return;
				}

				ArrowComponent arrow;
				arrowMap.TryGetValue(cellIndex, out arrow);
				newDirection = ShouldRedirect(cell, cellIndex, myDirection, arrow.Direction,
					gameConfig.DiminishesArrows);

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
		m_Buffer.AddJobHandleForProducer(job);
		return job;
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