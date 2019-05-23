using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZombieMaze {

	public class Maze : MonoBehaviour {
		
		public struct Tile {
			public bool leftWall;
			public bool upWall; // positive direction
			public bool rightWall;
			public bool downWall;
			public bool tempVisited;
		}

		public static Maze instance { get; private set; }

		public const float TILE_SPACING = 1;

		#region Inspector Properties

		public int initialWidth = 15;
		public int initialLength = 10;

		public int numMovingWalls = 5;
		public int movingWallDistanceMin = 2;
		public int movingWallDistanceMax = 10;
		[Tooltip("Min speed of a moving wall, in tiles/second.")]
		public float movingWallSpeedMin = .1f;
		[Tooltip("Max speed of a moving wall, in tiles/second.")]
		public float movingWallSpeedMax = 4;


		[Tooltip("Speed of the ball in tiles/second.")]
		public float ballSpeed = 1;

		public int numCapsules = 5;

		[Tooltip("Number of zombies spawned.")]
		public int numZombies = 5;

		[Tooltip("Speed of the zombies in tiles/second")]
		public float zombieSpeed = 2;

		[Tooltip("Min bound for delay after spawning before zombie can move.")]
		public float zombieStartDelayMin = 2;
		[Tooltip("Max bound for delay after spawning before zombie can move.")]
		public float zombieStartDelayMax = 10;

		[Tooltip("Distance between origins of ball and zombie for them to be considered touching")]
		public float zombieTouchDistance = 1;

		[Tooltip("Set to true to make the ball immune to the zombies.")]
		public bool ballInvincible = false;

		[Tooltip("Width of vertical open strips in the maze.")]
		public int openStripsWidth = 6;
		[Tooltip("Width in-between the vertical open strips.")]
		public int mazeStripsWidth = 4;


		public int movingWallSize = 1;



		[Header("Prefabs")]
		public GameObject tilePrefab;
		public GameObject wallPrefab;
		public GameObject movingWallPrefab;
		public GameObject capsulePrefab;
		public GameObject zombiePrefab;

		#endregion

		public int Width {
			get {
				return tiles.Count;
			}
		}
		public int Length {
			get {
				if (tiles.Count <= 0)
					return 0;
				return tiles[0].Count;
			}
		}
		public int NumTiles {
			get {
				return Width * Length;
			}
		}

		public Vector2Int HomeBase { get; private set; }


		/// <summary>
		/// Gets the number of tiles on the perimeter, if each edge of the border went inwards by the given border amount.
		/// </summary>
		public int NumBorderTiles(int border) {
			return (Width - border * 2) * 2 + (Length - border * 2) * 2 - 4;
		}

		/// <summary>
		/// Gets the accumulated number of tiles on the perimeter, totalling the amount of each border in [0, border]
		/// </summary>
		public int NumAccuBorderTiles(int border) {
			int ret = 0;
			for (int i = 0; i <= border; i++) {
				ret += NumBorderTiles(i);
			}
			return ret;
		}

		/// <summary>
		/// Gets the position of the indexth edge tile.  Border is automatically chosen, going downward.
		/// </summary>
		public Vector2Int GetEdgeTileAt(int index) {
			
			int border = 0;
			while (true) {
				int numBorderTiles = NumBorderTiles(border);
				// failsafe
				if (numBorderTiles <= 0) {
					return new Vector2Int(-1, -1);
				}
				if (index < numBorderTiles) {
					return GetEdgeTileAt(index, border);
				}
				index -= numBorderTiles;
				border++;
			}

		}

		/// <summary>
		/// Gets the position of the indexth edge tile (out of NumBorderTiles), of a perimeter of the maze where each edge went inwards by the border amount.
		/// </summary>
		/// <param name="index">Index in [0, NumBorderTiles(border)).</param>
		public Vector2Int GetEdgeTileAt(int index, int border){
			if (index < 0 || index >= NumBorderTiles(border))
				return new Vector2Int(-1, -1);

			int w = Width - border * 2;
			int l = Length - border * 2;

			Vector2Int ret = new Vector2Int(border, border);
			if (index < w) {
				ret += new Vector2Int(index, 0);
			} else if (index < w + l - 1) {
				ret += new Vector2Int(w - 1, index - w + 1);
			} else if (index < w + l + w - 2) {
				ret += new Vector2Int(w * 2 + l - 3 - index, l - 1);
			} else {
				// y + index = NumEdgeTiles
				ret += new Vector2Int(0, NumBorderTiles(border) - index);
			}

			return ret;
		}





		/// <summary>
		/// Gets tile at the given coordinates, or null if coordinates are invalid.
		/// </summary>
		public Tile? GetTile(int col, int row){
			if (col < 0 || col >= Width)
				return null;
			if (row < 0 || row >= Length)
				return null;
			return tiles[col][row];
		}

		/// <summary>
		/// Adds/removes up wall at [col][row], and adds/removes the down wall at tile [col][row + 1]
		/// </summary>
		public void SetUpWall(int col, int row, bool exist) {
			if (col < 0 || col >= Width)
				return;
			if (0 <= row && row < Length) {
				Tile tile = tiles[col][row];
				tile.upWall = exist;
				tiles[col][row] = tile;
			}
			if (0 <= row + 1 && row + 1 < Length) {
				Tile tile = tiles[col][row + 1];
				tile.downWall = exist;
				tiles[col][row + 1] = tile;
			}
		}

		/// <summary>
		/// Adds/removes down wall at [col][row], and adds/removes the up wall at tile [col][row - 1]
		/// </summary>
		public void SetDownWall(int col, int row, bool exist) {
			SetUpWall (col, row - 1, exist);
		}

		/// <summary>
		/// Adds/removes right wall at [col][row], and adds/removes the left wall at tile [col + 1][row]
		/// </summary>
		public void SetRightWall(int col, int row, bool exist) {
			if (row < 0 || row >= Length)
				return;
			if (0 <= col && col < Width) {
				Tile tile = tiles[col][row];
				tile.rightWall = exist;
				tiles[col][row] = tile;
			}
			if (0 <= col + 1 && col + 1 < Width) {
				Tile tile = tiles[col + 1][row];
				tile.leftWall = exist;
				tiles[col + 1][row] = tile;
			}
		}

		/// <summary>
		/// Adds/removes left wall at [col][row], and adds/removes the right wall at tile [col - 1][row]
		/// </summary>
		public void SetLeftWall(int col, int row, bool exist) {
			SetRightWall(col - 1, row, exist);
		}

		/// <summary>
		/// Resizes 2D tiles list to the given dimesnions.  Contents of tiles are not set.
		/// </summary>
		public void ResizeTiles(int width, int length){
			if (tiles.Count > width) {
				tiles.RemoveRange (width, tiles.Count - width);
			}
			while (tiles.Count < width) {
				tiles.Add(new List<Tile>());
			}

			foreach (List<Tile> column in tiles) {
				if (column.Count > length) {
					column.RemoveRange (length, column.Count - length);
				}
				while (column.Count < length) {
					column.Add(new Tile());
				}
			}
		}

		/// <summary>
		/// Gets position of a local transform of an object at the given col and row.
		/// </summary>
		/// <param name="yValue">y value in the returned Vector3.</param>
		public static Vector3 GetTransformPosition(float col, float row, float yValue = 0) {
			return new Vector3 (
				col * TILE_SPACING,
				yValue,
				row * TILE_SPACING
			);
		}

		/// <summary>
		/// Converts local position to tile coordinates.
		/// </summary>
		public static Vector2Int GetTilePosition(Vector3 localPosition){
			Vector2 tilePos = GetTilePositionF(localPosition);
			return new Vector2Int (
				Mathf.FloorToInt(tilePos.x),
				Mathf.FloorToInt(tilePos.y)
			);
		}

		/// <summary>
		/// Converts local position to tile coordinates as floats
		/// </summary>
		public static Vector2 GetTilePositionF(Vector3 localPosition){
			return new Vector2 (
				localPosition.x / TILE_SPACING,
				localPosition.z / TILE_SPACING
			);
		}

		public void CreateCapsules(int numCapsules) {

			// clear existing capsules
			foreach (Capsule capsule in capsules) {
				Destroy(capsule.gameObject);
			}
			capsules.Clear();

			// get list of possible locations
			List<int> emptyTiles = new List<int>(Width * Length);
			for (int i = 0; i < Width * Length; i++) {
				emptyTiles.Add(i);
			}
			emptyTiles.RemoveAt(HomeBase.x + HomeBase.y * Width); // remove home base location
			// shuffle list
			for (int i = 0; i < emptyTiles.Count; i++) 
			{
				int rand = Random.Range(0, emptyTiles.Count);
				int temp = emptyTiles[i];
				emptyTiles[i] = emptyTiles[rand];
				emptyTiles[rand] = temp;
			}

			numCapsules = Mathf.Min(numCapsules, emptyTiles.Count);
			for (int i = 0; i < numCapsules; i++) {
				Capsule capsule = Instantiate(capsulePrefab, transform).GetComponent<Capsule>();
				capsule.col = emptyTiles[i] % Width;
				capsule.row = emptyTiles[i] / Width;
				capsule.UpdatePosition();
				capsules.Add(capsule);
			}
			emptyTiles.Clear();

		}

		public void ResetCapsules(){
			foreach (Capsule capsule in capsules) {
				capsule.Reset();
			}
		}

		/// <summary>
		/// Gets capsule at specified position.  Includes captured capsules.  Returns null if no capsule exists there.
		/// </summary>
		public Capsule GetCapsule(int col, int row) {
			foreach (Capsule capsule in capsules) {
				if (capsule.col == col && capsule.row == row)
					return capsule;
			}
			return null;
		}

		public Capsule GetCapsuleByIndex(int index) {
			if (index < 0 || index >= capsules.Count)
				return null;
			return capsules[index];
		}

		public int numCapsulesInGame {
			get { return capsules.Count; }
		}

		public bool AllCapsulesCaptured() {
			foreach (Capsule capsule in capsules) {
				if (!capsule.captured)
					return false;
			}
			return true;
		}

		public void CreateZombies(int numZombies) {

			// clear existing zombies
			foreach (Zombie zombie in zombies) {
				Destroy(zombie.gameObject);
			}
			zombies.Clear();

			// get enough borders to fit the zombies
			int numBorders = 1;
			int numAccuBorderTiles = 0;
			while (true) {
				numAccuBorderTiles = NumAccuBorderTiles(numBorders - 1);
				if (numBorders >= Mathf.Min(Width / 2, Length / 2))
					break;
				if (numZombies < numAccuBorderTiles)
					break;
				numBorders++;
			}

			numZombies = Mathf.Min(numZombies, numAccuBorderTiles);

			// list of possible positions
			List<int> edgeIndices = new List<int>();
			for (int i = 0; i < numAccuBorderTiles; i++) {
				edgeIndices.Add(i);
			}
			// shuffle list
			for (int i = 0; i < edgeIndices.Count; i++) 
			{
				int rand = Random.Range(0, edgeIndices.Count);
				int temp = edgeIndices[i];
				edgeIndices[i] = edgeIndices[rand];
				edgeIndices[rand] = temp;
			}

			for (int i = 0; i < numZombies; i++) {
				// create zombie
				Vector2Int pos = GetEdgeTileAt(edgeIndices[i]);

				Zombie zombie = Instantiate(zombiePrefab, transform).GetComponent<Zombie>();
				zombies.Add(zombie);
				zombie.moveSpeed = zombieSpeed;

				// zombie targets ball or one of the capsules
				int capsuleIndex = (i % (capsules.Count + 1)) - 1;
				if (capsuleIndex == -1) {
					zombie.TargetBall();
				} else {
					zombie.TargetPosition(capsules[capsuleIndex].col, capsules[capsuleIndex].row);
				}

				zombie.DropIn(pos.x, pos.y);
				zombie.startDelayDuration = Random.Range(zombieStartDelayMin, zombieStartDelayMax);
			}


		}

		public void GenerateMaze(int width, int length) {
			
			// resetting grid.  Every tile has all walls
			ClearGameObjects();
			ResizeTiles(width, length);
			Tile tile = new Tile();
			tile.leftWall = true;
			tile.upWall = true;
			tile.rightWall = true;
			tile.downWall = true;
			tile.tempVisited = false;
			for (int c = 0; c < width; c++) {
				for (int r = 0; r < length; r++) {
					tiles[c][r] = tile;
				}
			}

			// pick home base
			HomeBase = new Vector2Int(Random.Range(0, Width), Random.Range(0, Length));

			// generating maze with recursive backtracker algorithm - https://en.wikipedia.org/wiki/Maze_generation_algorithm
			Stack<Vector2Int> stack = new Stack<Vector2Int>();
			Vector2Int current = new Vector2Int (Random.Range (0, width), Random.Range (0, length));
			Tile temp = tiles [current.x] [current.y];
			temp.tempVisited = true;
			tiles[current.x][current.y] = temp;
			int numVisited = 1;

			while (numVisited < NumTiles) {
				// choose random adjacent unvisited tile
				List<Vector2Int> unvisitedNeighbors = new List<Vector2Int> ();
				if (current.x > 0 && !tiles[current.x - 1][current.y].tempVisited)
					unvisitedNeighbors.Add(new Vector2Int(current.x - 1, current.y));
				if (current.y > 0 && !tiles[current.x][current.y - 1].tempVisited)
					unvisitedNeighbors.Add(new Vector2Int(current.x, current.y - 1));
				if (current.x < width - 1 && !tiles[current.x + 1][current.y].tempVisited)
					unvisitedNeighbors.Add(new Vector2Int(current.x + 1, current.y));
				if (current.y < length - 1 && !tiles[current.x][current.y + 1].tempVisited)
					unvisitedNeighbors.Add(new Vector2Int(current.x, current.y + 1));

				if (unvisitedNeighbors.Count > 0) {
					// visit neighbor
					Vector2Int next = unvisitedNeighbors[Random.Range(0, unvisitedNeighbors.Count)];
					stack.Push (current);
					// remove wall between tiles
					Tile currentTile = tiles[current.x][current.y];
					Tile nextTile = tiles[next.x][next.y];
					if (next.x > current.x) {
						currentTile.rightWall = false;
						nextTile.leftWall = false;
					} else if (next.y > current.y) {
						currentTile.upWall = false;
						nextTile.downWall = false;
					} else if (next.x < current.x) {
						currentTile.leftWall = false;
						nextTile.rightWall = false;
					} else {
						currentTile.downWall = false;
						nextTile.upWall = false;
					}
					nextTile.tempVisited = true;
					numVisited++;
					tiles[current.x][current.y] = currentTile;
					tiles[next.x][next.y] = nextTile;
					current = next;
				} else {
					// backtrack if no unvisited neighboring tiles
					if (stack.Count > 0) {
						current = stack.Pop();
					}
				}
			}

			// make moving walls and clear walls that would be in the way
			int loopCount = 0;
			for (int i = 0; i < numMovingWalls; i++) {

				// break if can't find enough space for moving walls
				loopCount++;
				if (loopCount > 9999)
					break;

				MovingWall movingWall = Instantiate(movingWallPrefab, transform).GetComponent<MovingWall>();
				movingWall.horizontal = true;//Random.value < .5f;

				if (movingWall.horizontal) {
					movingWall.size = Mathf.Min(movingWallSize, width - movingWallDistanceMin);
				} else {
					movingWall.size = Mathf.Min(movingWallSize, length - movingWallDistanceMin);
				}

				if (movingWall.horizontal) {
					movingWall.distance = Random.Range(Mathf.Min(movingWallDistanceMin, width - movingWall.size + 1), Mathf.Min(movingWallDistanceMax, width - movingWall.size + 1) + 1);
					movingWall.col = Random.Range(0, width - movingWall.distance + 1 - movingWall.size + 1);
					movingWall.row = Random.Range(1, length);
				} else {
					movingWall.distance = Random.Range(Mathf.Min(movingWallDistanceMin, length - movingWall.size + 1), Mathf.Min(movingWallDistanceMax, length - movingWall.size + 1) + 1);
					movingWall.col = Random.Range(1, width);
					movingWall.row = Random.Range(0, length - movingWall.distance + 1 - movingWall.size + 1);
				}
				movingWall.speed = Random.Range(movingWallSpeedMin, movingWallSpeedMax);

				// check that wall doesn't overlap with other walls (can intersect though)
				bool overlaps = false;
				foreach (MovingWall other in movingWalls) {
					if (MovingWall.Overlap (other, movingWall)) {
						overlaps = true;
						break;
					}
				}
				if (overlaps) {
					Destroy(movingWall.gameObject);
					i--;
					continue;
				}

				movingWall.SetRandomIndex();
				movingWall.RemoveWallsInPath();
				movingWall.UpdateTransform();

				movingWalls.Add(movingWall);
			}
			numMovingWalls = movingWalls.Count;



			// remove vertical strips of walls
			if (openStripsWidth + mazeStripsWidth > 0) {
				int offset = 0; // (aligns with border width)
				for (; offset < width; offset += openStripsWidth + mazeStripsWidth) {

					for (int c = offset; c < Mathf.Min(offset + openStripsWidth, width); c++) {

						for (int r = 0; r < length; r++) {

							if (r > 0) {
								SetDownWall(c, r, false);
							}
							if (c > offset) {
								SetLeftWall(c, r, false);
							}

						}

					}

				}
			}

			// remove walls at left vertical border
			for (int c = 0; c < Mathf.Min(openStripsWidth, width); c++) {
				for (int r = 0; r < length; r++) {
					if (r > 0) {
						SetDownWall(c, r, false);
					}
					if (c > 0) {
						SetLeftWall(c, r, false);
					}
				}
			}
			// remove walls at right vertical border
			for (int c = Mathf.Max(width - openStripsWidth, 0); c < width; c++) {
				for (int r = 0; r < length; r++) {
					if (r > 0) {
						SetDownWall(c, r, false);
					}
					if (c > 0) {
						SetLeftWall(c, r, false);
					}
				}
			}

			// remove walls at bottom horizontal border
			for (int r = 0; r < Mathf.Min(openStripsWidth, length); r++) {
				for (int c = 0; c < width; c++) {
					if (r > 0) {
						SetDownWall(c, r, false);
					}
					if (c > 0) {
						SetLeftWall(c, r, false);
					}
				}
			}
			// remove walls at top horizontal border
			for (int r = Mathf.Max(length - openStripsWidth, 0); r < length; r++) {
				for (int c = 0; c < width; c++) {
					if (r > 0) {
						SetDownWall(c, r, false);
					}
					if (c > 0) {
						SetLeftWall(c, r, false);
					}
				}
			}





			// instantiate gameObjects

			GameObject groundTile = Instantiate(tilePrefab, transform);
			groundTile.transform.localPosition = new Vector3(TILE_SPACING * (width - 1) / 2, groundTile.transform.localPosition.y - .01f, TILE_SPACING * (length - 1) / 2);
			groundTile.transform.localScale = new Vector3(TILE_SPACING * width, groundTile.transform.localScale.y, TILE_SPACING * length);
			tileGameObjects.Add(groundTile);


			for (int c = 0; c < width; c++) {
				for (int r = 0; r < length; r++) {

					tile = tiles[c][r];

					// add tile, but only if it's home base
					if (c == HomeBase.x && r == HomeBase.y) {

						GameObject tileGO = Instantiate(tilePrefab, transform);
						tileGO.transform.localPosition = new Vector3(TILE_SPACING * c, tileGO.transform.localPosition.y, TILE_SPACING * r);
						tileGameObjects.Add(tileGO);

						tileGO.GetComponent<ZombieMaze.Tile>().IsHomeBase = true;
					}


					GameObject wallGO;
					if (tile.leftWall) {
						wallGO = Instantiate (wallPrefab, transform);
						wallGO.transform.localPosition = new Vector3 (
							TILE_SPACING * (c - .5f),
							wallGO.transform.localPosition.y,
							TILE_SPACING * r
						);
						wallGO.transform.localRotation = Quaternion.Euler (0, 90, 0);
						wallGameObjects.Add (wallGO);
					}
					if (tile.downWall) {
						wallGO = Instantiate (wallPrefab, transform);
						wallGO.transform.localPosition = new Vector3 (
							TILE_SPACING * c,
							wallGO.transform.localPosition.y,
							TILE_SPACING * (r - .5f)
						);
						wallGO.transform.localRotation = Quaternion.Euler (0, 0, 0);
						wallGameObjects.Add (wallGO);
					}
					if (c == width - 1 && tile.rightWall) {
						wallGO = Instantiate (wallPrefab, transform);
						wallGO.transform.localPosition = new Vector3 (
							TILE_SPACING * (c + .5f),
							wallGO.transform.localPosition.y,
							TILE_SPACING * r
						);
						wallGO.transform.localRotation = Quaternion.Euler (0, 90, 0);
						wallGameObjects.Add (wallGO);
					}
					if (r == length - 1 && tile.upWall) {
						wallGO = Instantiate (wallPrefab, transform);
						wallGO.transform.localPosition = new Vector3 (
							TILE_SPACING * c,
							wallGO.transform.localPosition.y,
							TILE_SPACING * (r + .5f)
						);
						wallGO.transform.localRotation = Quaternion.Euler (0, 0, 0);
						wallGameObjects.Add (wallGO);
					}

				}
			}

			CreateCapsules(numCapsules);

			CreateZombies(numZombies);


			Camera.main.GetComponent<CameraControl>().InitialSetUp();

		}

		void ClearGameObjects() {
			foreach (GameObject tileGO in tileGameObjects) {
				Destroy(tileGO);
			}
			tileGameObjects.Clear();
			foreach (GameObject wallGO in wallGameObjects) {
				Destroy(wallGO);
			}
			wallGameObjects.Clear();
			foreach (MovingWall movingWall in movingWalls) {
				Destroy(movingWall.gameObject);
			}
			movingWalls.Clear();
			foreach (Capsule capsule in capsules) {
				Destroy(capsule.gameObject);
			}
			capsules.Clear();
			foreach (Zombie zombie in zombies) {
				Destroy(zombie.gameObject);
			}
			zombies.Clear();
		}

		void Awake() {
			if (instance != null) {
				Destroy(gameObject);
				return;
			}
			instance = this;
		}

		// Use this for initialization
		void Start () {

			GenerateMaze(initialWidth, initialLength);

			Ball.instance.MoveToHomeBase();

		}
		
		// Update is called once per frame
		void Update () {
			
		}

		void OnDestroy() {
			if (instance == this) {
				instance = null;
			}
			ClearGameObjects();
		}


		private List<List<Tile>> tiles= new List<List<Tile>>();
		private List<GameObject> tileGameObjects = new List<GameObject>();
		private List<GameObject> wallGameObjects = new List<GameObject>();
		private List<MovingWall> movingWalls = new List<MovingWall>();
		private List<Capsule> capsules = new List<Capsule>();
		private List<Zombie> zombies = new List<Zombie>();

	}

}