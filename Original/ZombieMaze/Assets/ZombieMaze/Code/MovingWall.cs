using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZombieMaze {

	public class MovingWall : MonoBehaviour {

		/// <summary>
		/// Moving horizontally or vertically.
		/// If horizontal, will replace [col + i][row - 1]'s up walls and [col + i][row]'s down walls.
		/// If vertical, will replace [col - 1][row + i]'s right walls and [col][row + i]'s left walls.
		/// </summary>
		public bool horizontal;
		/// <summary>
		/// x (in tiles) of start position
		/// </summary>
		public int col;
		/// <summary>
		/// z (in tiles) of start position
		/// </summary>
		public int row;
		/// <summary>
		/// How far (in tiles) the wall moves.
		/// </summary>
		public int distance = 1;

		/// <summary>
		/// The size of the wall (in tiles).
		/// </summary>
		public int size = 2;

		/// <summary>
		/// How many tiles the wall moves in one second.
		/// </summary>
		public float speed = .5f;

		/// <summary>
		/// Position past the start in wall movement
		/// </summary>
		public int index {
			get {
				return Mathf.FloorToInt(indexF);
			}
		}
		/// <summary>
		/// true: position going up.  false (pong): position going down.
		/// </summary>
		public bool ping { get; set; }

		/// <summary>
		/// Sets index to be random inside length.
		/// </summary>
		public void SetRandomIndex() {
			indexF = Random.Range(0f, distance);
			ping = Random.value < .5f;
		}

		public void RemoveWallsInPath() {
			if (horizontal) {
				for (int i = 0; i < distance + size - 1; i++) {
					Maze.instance.SetUpWall(col + i, row - 1, false);
				}
			} else {
				for (int i = 0; i < distance + size - 1; i++) {
					Maze.instance.SetRightWall(col - 1, row + i, false);
				}
			}
		}

		/// <summary>
		/// Removes all walls in the maze in the moving wall's path, and adds a wall at its position.
		/// </summary>
		public void UpdateMazePresence() {

			if (horizontal) {
				for (int i = 0; i < distance + size - 1; i++) {
					Maze.instance.SetUpWall(col + i, row - 1, index <= i && i < index + size);
				}
			} else {
				for (int i = 0; i < distance + size - 1; i++) {
					Maze.instance.SetRightWall(col - 1, row + i, index <= i && i < index + size);
				}
			}

		}

		public void UpdateTransform() {

			transform.localScale = new Vector3(size, transform.localScale.y, transform.localScale.z);

			if (horizontal) {

				transform.localPosition = new Vector3 (
					Maze.TILE_SPACING * (col + index + .5f * (size - 1)),
					transform.localPosition.y,
					Maze.TILE_SPACING * (row - .5f)
				);
				transform.localRotation = Quaternion.Euler(0, 0, 0);

			} else {

				transform.localPosition = new Vector3 (
					Maze.TILE_SPACING * (col - .5f),
					transform.localPosition.y,
					Maze.TILE_SPACING * (row + index + .5f * (size - 1))
				);
				transform.localRotation = Quaternion.Euler(0, 90, 0);

			}

		}

		/// <summary>
		/// Returns if the paths of the given moving walls overlap.
		/// This can only be true if the walls are parallel and in the same col/row.
		/// </summary>
		public static bool Overlap(MovingWall mw1, MovingWall mw2) {
			if (mw1.horizontal != mw2.horizontal)
				return false;

			if (mw1.horizontal) {
				if (mw1.row != mw2.row)
					return false;

				return mw1.col < mw2.col + mw2.distance + mw2.size - 1 &&
					mw2.col < mw1.col + mw1.distance + mw1.size - 1;
			} else {
				if (mw1.col != mw2.col)
					return false;

				return mw1.row < mw2.row + mw2.distance + mw2.size - 1 &&
					mw2.row < mw1.row + mw1.distance + mw1.size - 1;
			}

		}

		private float indexF = 0;

		// Use this for initialization
		void Start () {
			
		}
		
		// Update is called once per frame
		void Update () {

			// move wall
			if (ping) {
				indexF += Time.deltaTime * speed;
				if (indexF >= distance) {
					indexF = distance - (indexF - distance);
					ping = false;
				}
			} else {
				indexF -= Time.deltaTime * speed;
				if (indexF < 0) {
					indexF *= -1;
					ping = true;
				}
			}
			// failsafe to keep within bounds
			indexF -= Mathf.Floor(indexF / distance) * distance;

			UpdateMazePresence();
			UpdateTransform();

		}


	}

}