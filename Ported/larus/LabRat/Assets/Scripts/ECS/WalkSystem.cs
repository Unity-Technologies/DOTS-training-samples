using ECSExamples;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class WalkSystem : ComponentSystem
{
	private Board m_Board;
	//private EntityQuery m_BoardQuery;
	private EntityQuery m_ConfigQuery;

	protected override void OnCreate()
	{
		m_Board = GameObject.FindObjectOfType<Board>();
		//m_BoardQuery = GetEntityQuery(typeof(BoardDataComponent));
		m_ConfigQuery = GetEntityQuery(typeof(GameConfigComponent));
	}

	//protected override JobHandle OnUpdate(JobHandle inputDep)
	protected override void OnUpdate()
	{
		var gameConfig = m_ConfigQuery.GetSingleton<GameConfigComponent>();
		//var board = m_BoardQuery.GetSingleton<BoardDataComponent>();
		//var cellData = EntityManager.GetBuffer<CellComponent>(m_BoardQuery.GetSingletonEntity());

		// TODO: Add WalkingState component to entity by default
		//var job =
		//Entities.ForEach((Entity entity, in ref WalkComponent walker, ref Translation position, ref Rotation rotation) =>
		Entities.ForEach((Entity entity, ref WalkComponent walker, ref Translation position, ref Rotation rotation) =>
		{
			//float3 fwd = new float3(0, 0, 1);
			float3 fwd = new float3(0, 0, walker.Speed);
			fwd += math.mul(rotation.Value, fwd);
			//float3 fwd = ForwardVectorFromRotation(rotation.Value);

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
					{
						newPos.z = Mathf.Lerp(newPos.z, Mathf.Round(newPos.z), speed);
					}
					else if (Mathf.Abs(fwd.z) > 0)
					{
						newPos.x = Mathf.Lerp(newPos.x, Mathf.Round(newPos.x), speed);
					}

					Debug.Log("Return to whole number " + position.Value + " -> " + newPos);
					position.Value = newPos;
				}

				// Apply walk deltas in a loop so that even if we have a super low framerate, we
				// don't skip cells in the board.
				while (true)
				{
					float slice = Mathf.Min(m_Board.boardDesc.cellSize.x * 0.3f, remainingDistance);
					remainingDistance -= slice;
					if (slice <= 0f || Mathf.Approximately(0f, slice))
					{
						Debug.Log("No change");
						break;
					}

					var delta = fwd * slice;
					newPos.x += delta.x;
					newPos.y += delta.y;
					newPos.z += delta.z;
					//var myDirection = Walks.DirectionFromVector(fwd);
					var myDirection = DirectionFromRotation(rotation.Value);

					// TODO: seems this always resets position to ~0,0,0
					// Round position values for checking the board. This is so that
					// we collide with arrows and walls at the right time.
					/*switch (myDirection)
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
					}*/
					position.Value = newPos;

					var vect = new Vector3 {x = position.Value.x, y = position.Value.y, z = position.Value.z};
					var cell = m_Board.CellAtWorldPosition(vect);

					// null == falling
					if (cell == null)
					    return;

					var newDirection = cell.ShouldRedirect(myDirection, gameConfig.DiminishesArrows);

					/*var newDirection = myDirection;
					for (int i = 0; i < cellData.Length; ++i)
					{
						if ((cellData[i].data & CellData.WallNorth) == CellData.WallNorth)
							newDirection = Direction.North;
						else if ((cellData[i].data & CellData.WallSouth) == CellData.WallSouth)
							newDirection = Direction.South;
						else if ((cellData[i].data & CellData.WallEast) == CellData.WallEast)
							newDirection = Direction.East;
						else if ((cellData[i].data & CellData.WallWest) == CellData.WallWest)
							newDirection = Direction.West;
					}*/

					Debug.Log("Forward=" + fwd + " oldDir=" + myDirection + " newDir="+ newDirection);
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
			return Direction.East;
		return Direction.North;
	}
}