using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZombieMaze {

	/// <summary>
	/// Base class for Ball and Zombie that handles movement in the Maze.
	/// </summary>
	public class Character : MonoBehaviour {
		
		public int col {
			get {
				return Mathf.RoundToInt(colF);
			}
		}
		public int row {
			get {
				return Mathf.RoundToInt(rowF);
			}
		}

		public float colF {
			get;
			protected set;
		}
		public float rowF {
			get;
			protected set;
		}

		public virtual float moveSpeed { get; set; }

		public enum MoveState {
			IDLE,
			MOVING,
		}
		public MoveState moveState { get; private set; }
		protected float time = 0;

		public virtual void UpdateLocalPosition() {
			transform.localPosition = Maze.GetTransformPosition(colF, rowF, transform.localPosition.y);
		}

		public enum Direction {
			NONE,
			LEFT,
			UP,
			RIGHT,
			DOWN
		}

		/// <summary>
		/// Stops character immediately.  If in the middle of moving, character gets repositioned at where it was before it began moving.
		/// </summary>
		public void Stop(){
			if (moveState == MoveState.MOVING) {
				colF = moveStartPos.x;
				rowF = moveStartPos.y;
			}
			moveState = MoveState.IDLE;
			time = 0;
		}

		/// <summary>
		/// Moves character in the given direction.  Only works when moveState is IDLE.
		/// </summary>
		public void MoveDirection(Direction direction) {
			if (moveState != MoveState.IDLE)
				return;

			Maze.Tile? tileN = Maze.instance.GetTile(col, row);
			if (!tileN.HasValue)
				return;
			Maze.Tile tile = tileN.Value;

			switch (direction) {
			case Direction.LEFT:

				if (tile.leftWall)
					return;
				moveDirection.Set(-1, 0);

				break;
			case Direction.DOWN:

				if (tile.downWall)
					return;
				moveDirection.Set(0, -1);

				break;
			case Direction.RIGHT:

				if (tile.rightWall)
					return;
				moveDirection.Set(1, 0);

				break;
			case Direction.UP:

				if (tile.upWall)
					return;
				moveDirection.Set(0, 1);

				break;
			}
			moveStartPos.Set(col, row);

			moveState = MoveState.MOVING;
			time = 0;

		}

		protected virtual void Update(){
			
			time += Time.deltaTime;
			switch (moveState) {
			case MoveState.IDLE:
				
				break;
			case MoveState.MOVING:
				// locked in to moving in new direction
				if (moveSpeed != 0 && time >= 1 / moveSpeed) {
					colF = moveStartPos.x + moveDirection.x;
					rowF = moveStartPos.y + moveDirection.y;
					moveState = MoveState.IDLE;

				} else {
					colF = moveStartPos.x + moveDirection.x * time * moveSpeed;
					rowF = moveStartPos.y + moveDirection.y * time * moveSpeed;
				}
				break;
			}
			UpdateLocalPosition();

		}

		protected Vector2Int moveStartPos = new Vector2Int();
		protected Vector2Int moveDirection = new Vector2Int();


	}

}