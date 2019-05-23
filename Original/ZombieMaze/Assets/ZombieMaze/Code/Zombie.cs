using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZombieMaze {

	[RequireComponent(typeof(MeshRenderer))]
	public class Zombie : Character {

		public float dropHeight = 1;
		public float dropDuration = 2;
		public Color color1 = Color.green;
		public Color color2 = Color.black;

		public enum Target {
			NONE,
			BALL,
			POSITION
		}
		public Target target { get; private set; }
		public Vector2Int targetPosition { get; private set; }

		public bool dropping { get; private set; }

		public bool startDelay { get; private set; }
		public float startDelayDuration { get; set; }

		public MeshRenderer meshRenderer { get; private set; }



		public void DropIn(int col, int row) {
			if (dropping)
				return;
			Stop();
			colF = col;
			rowF = row;
			dropping = true;
			dropTime = 0;
			startDelay = true;
			startDelayTime = 0;
		}

		public void DropInAtRandomEdge() {
			Vector2Int pos = Maze.instance.GetEdgeTileAt(Random.Range(0, Maze.instance.NumBorderTiles(0)), 0);
			DropIn(pos.x, pos.y);
		}

		public void TargetBall(){
			target = Target.BALL;
		}

		public void TargetPosition(int col, int row){
			target = Target.POSITION;
			targetPosition = new Vector2Int(col, row);
		}

		public override void UpdateLocalPosition()
		{
			float y = defaultY;
			if (dropping) {
				y = Mathf.Lerp(defaultY + dropHeight, defaultY, dropTime / dropDuration);
			}
			transform.localPosition = Maze.GetTransformPosition(colF, rowF, y);
		}

		float defaultY = 0;
		float dropTime = 0;

		public bool isTouchingBall {
			get {
				Vector2 p1 = new Vector2(transform.localPosition.x, transform.localPosition.z);
				Vector2 p2 = new Vector2(Ball.instance.transform.localPosition.x, Ball.instance.transform.localPosition.z);
				return (p1 - p2).sqrMagnitude <= Maze.instance.zombieTouchDistance * Maze.instance.zombieTouchDistance;
			}
		}

		void Awake(){
			meshRenderer = GetComponent<MeshRenderer>();
		}

		// Use this for initialization
		void Start () {
			defaultY = transform.localPosition.y;
			InitializeSearchGrid();

			meshRenderer.material.color = Color.Lerp(color1, color2, Random.value);

		}
		
		// Update is called once per frame
		protected override void Update() {

			dropTime += Time.deltaTime;
			if (dropping) {
				if (dropTime >= dropDuration) {
					dropping = false;
				}
			} else if (startDelay) {
				startDelayTime += Time.deltaTime;
				if (startDelayTime >= startDelayDuration) {
					startDelay = false;
				}
			} else {

				if (moveState == MoveState.IDLE && target != Target.NONE) {
					// decide where to move

					SearchMaze();

					// get path from target back to zombie
					Direction direction = Direction.NONE;
					if (target == Target.BALL) {
						targetPosition = new Vector2Int(Ball.instance.col, Ball.instance.row);
					}
					Vector2Int pos = targetPosition;
					Cell cell = searchGrid[pos.x][pos.y];
					while (cell.visited && !(pos.x == col && pos.y == row)) {
						direction = cell.direction;

						switch (direction) {
						case Direction.LEFT:
							pos.x--;
							break;
						case Direction.DOWN:
							pos.y--;
							break;
						case Direction.RIGHT:
							pos.x++;
							break;
						case Direction.UP:
							pos.y++;
							break;
						}

						cell = searchGrid[pos.x][pos.y];
					}
					if (!cell.visited)
						direction = Direction.NONE;

					// move in opposite of last direction of the path
					if (direction != Direction.NONE) {
						switch (direction) {
						case Direction.DOWN:
							direction = Direction.UP;
							break;
						case Direction.UP:
							direction = Direction.DOWN;
							break;
						case Direction.LEFT:
							direction = Direction.RIGHT;
							break;
						case Direction.RIGHT:
							direction = Direction.LEFT;
							break;
						}
						MoveDirection(direction);
					}

				}

				// hitting ball
				if (isTouchingBall) {

					DropInAtRandomEdge();

					if (!Maze.instance.ballInvincible) {
						Ball.instance.MoveToHomeBase();
					}

				} else {

					// reaching target position
					if (target == Target.POSITION && col == targetPosition.x && row == targetPosition.y) {

						// pick new target position
						int index = Random.Range(-1, Maze.instance.numCapsulesInGame);
						if (index == -1) {
							TargetBall();
						} else {
							Capsule capsule = Maze.instance.GetCapsuleByIndex(index);
							TargetPosition(capsule.col, capsule.row);
						}

					}
				}

			}

			base.Update();

		}

		void OnDestroy(){
			foreach (List<Cell> column in searchGrid) {
				column.Clear();
			}
			searchGrid.Clear();
		}
		float startDelayTime = 0;

		/// <summary>
		/// Initializes the search grid to match dimensions of the maze.
		/// </summary>
		public void InitializeSearchGrid() {

			foreach (List<Cell> column in searchGrid) {
				column.Clear();
			}
			searchGrid.Clear();

			for (int c = 0; c < Maze.instance.Width; c++) {
				List<Cell> column = new List<Cell>();
				for (int r = 0; r < Maze.instance.Length; r++) {
					column.Add(new Cell());
				}
				searchGrid.Add(column);
			}

		}

		private struct Cell {
			public bool visited;
			public int distance;
			public Direction direction;
		}

		private List<List<Cell>> searchGrid = new List<List<Cell>>();

		/// <summary>
		/// Searches the maze, marking every tile with the distance away and direction towards the shortest path back to this zombie.
		/// </summary>
		private void SearchMaze() {

			// clear cells
			Cell cell = new Cell();
			cell.visited = false;
			for (int c = 0; c < searchGrid.Count; c++) {
				for (int r = 0; r < searchGrid[c].Count; r++) {
					searchGrid[c][r] = cell;
				}
			}

			// failsafe (off grid)
			if (!Maze.instance.GetTile(col, row).HasValue)
				return;

			Queue<Vector2Int> cellsToCheck = new Queue<Vector2Int> ();
			Maze.Tile tile;
			List<Vector2Int> neighbors = new List<Vector2Int> ();

			// set first cell
			Vector2Int current = new Vector2Int(col, row);
			Cell currentCell = new Cell();
			currentCell.visited = true;
			currentCell.direction = Direction.NONE;
			currentCell.distance = 0;
			searchGrid[current.x][current.y] = currentCell;
			cellsToCheck.Enqueue(current);

			Cell neighborCell;
			while (cellsToCheck.Count > 0) {

				current = cellsToCheck.Dequeue();
				currentCell = searchGrid[current.x][current.y];
				tile = Maze.instance.GetTile(current.x, current.y).Value;

				// get neighbors
				neighbors.Clear();
				if (current.x > 0 && !tile.leftWall) {
					neighbors.Add(new Vector2Int(current.x - 1, current.y));
				}
				if (current.y > 0 && !tile.downWall) {
					neighbors.Add(new Vector2Int(current.x, current.y - 1));
				}
				if (current.x < Maze.instance.Width - 1 && !tile.rightWall) {
					neighbors.Add(new Vector2Int(current.x + 1, current.y));
				}
				if (current.y < Maze.instance.Length - 1 && !tile.upWall) {
					neighbors.Add(new Vector2Int(current.x, current.y + 1));
				}

				// check neighbors
				foreach (Vector2Int neighbor in neighbors) {

					neighborCell = searchGrid[neighbor.x][neighbor.y];

					// update neighbor with shortest path
					if (!neighborCell.visited || neighborCell.distance > currentCell.distance + 1) {
						if (neighbor.x < current.x) {
							neighborCell.direction = Direction.RIGHT;
						} else if (neighbor.y < current.y) {
							neighborCell.direction = Direction.UP;
						} else if (neighbor.x > current.x) {
							neighborCell.direction = Direction.LEFT;
						} else if (neighbor.y > current.y) {
							neighborCell.direction = Direction.DOWN;
						}
						neighborCell.distance = currentCell.distance + 1;
					}

					// visit neighbor
					if (!searchGrid[neighbor.x][neighbor.y].visited) {
						neighborCell.visited = true;
						cellsToCheck.Enqueue(neighbor);
					}

					searchGrid[neighbor.x][neighbor.y] = neighborCell;

				}

			}


		}


	}

}