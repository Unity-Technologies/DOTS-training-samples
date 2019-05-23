using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ECSExamples {

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

}
