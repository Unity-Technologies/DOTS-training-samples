using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JumpTheGun {
	
	public class TerrainArea : MonoBehaviour {

		[Header("Prefabs")]
		public GameObject boxPrefab;
		public GameObject tankPrefab;

		public static TerrainArea instance { get; private set; }

		public int width {
			get {
				return boxes.Count;
			}
		}
		public int length {
			get {
				if (boxes.Count == 0)
					return 0;
				return boxes[0].Count;
			}
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
			
		}
		
		// Update is called once per frame
		void Update () {
			
		}

		public Vector2Int BoxFromLocalPosition(Vector3 localPos){
			return new Vector2Int(Mathf.RoundToInt(localPos.x / Box.SPACING), Mathf.RoundToInt(localPos.z / Box.SPACING));
		}

		public Vector3 LocalPositionFromBox(int col, int row, float yPosition = 0){
			return new Vector3(col * Box.SPACING, yPosition, row * Box.SPACING);
		}

		public Box GetBox(int col, int row){
			if (col < 0 || col >= width)
				return null;
			if (row < 0 || row >= length)
				return null;
			return boxes[col][row];
		}

		/// <summary>
		/// Checks if the given cube intersects nearby boxes or tanks.
		/// </summary>
		public bool HitsCube(Vector3 center, float width) {

			// check nearby boxes
			int colMin = Mathf.FloorToInt((center.x - width / 2) / Box.SPACING);
			int colMax = Mathf.CeilToInt((center.x + width / 2) / Box.SPACING);
			int rowMin = Mathf.FloorToInt((center.z - width / 2) / Box.SPACING);
			int rowMax = Mathf.CeilToInt((center.z + width / 2) / Box.SPACING);

			colMin = Mathf.Max(0, colMin);
			colMax = Mathf.Min(this.width - 1, colMax);
			rowMin = Mathf.Max(0, rowMin);
			rowMax = Mathf.Min(length - 1, rowMax);

			for (int c = colMin; c <= colMax; c++) {
				for (int r = rowMin; r <= rowMax; r++) {
					if (boxes[c][r].HitsCube(center, width))
						return true;
				}
			}

			// TODO: check tanks

			return false;

		}

		/// <summary>
		/// Creates the given number of tanks, placing them randomly in the terrain.  No two tanks share the same spot.
		/// Returns a random point on the terrain that is not occupied by a tank.
		/// </summary>
		public Vector2Int CreateTanks(int numTanks) {

			ClearTanks();

			if (width <= 0 || length <= 0)
				return new Vector2Int(0, 0);
			
			// get list of possible locations
			List<int> emptyBoxes = new List<int>(width * length);
			for (int i = 0; i < width * length; i++) {
				emptyBoxes.Add(i);
			}
			ShuffleList(emptyBoxes);

			numTanks = Mathf.Min(numTanks, emptyBoxes.Count - 1);
			for (int i = 0; i < numTanks; i++) {
				int col = emptyBoxes[i] % width;
				int row = emptyBoxes[i] / width;

				Tank tank = Instantiate(tankPrefab, transform).GetComponent<Tank>();
				tanks.Add(tank);
				tank.SetTank(boxes[col][row]);

			}

			// return next point as a random unoccupied box
			return new Vector2Int(emptyBoxes[numTanks] % width, emptyBoxes[numTanks] / width);

		}

		public bool OccupiedBox(int col, int row){
			foreach (Tank tank in tanks) {
				if (tank.box.col == col && tank.box.row == row)
					return true;
			}
			return false;
		}


		public void CreateBoxes(int width, int length){
			ClearBoxes();

			for (int c = 0; c < width; c++) {
				List<Box> column = new List<Box>();
				for (int r = 0; r < length; r++) {
					float height = Random.Range(Game.instance.minTerrainHeight, Game.instance.maxTerrainHeight);
					Box box = Instantiate(boxPrefab, transform).GetComponent<Box>();
					box.SetBox(c, r, height);
					column.Add(box);
				}
				boxes.Add(column);
			}
		}

		public void ClearBoxes() {
			foreach (List<Box> column in boxes) {
				foreach (Box box in column) {
					Destroy(box.gameObject);
				}
				column.Clear();
			}
			boxes.Clear();
		}

		public void ClearTanks() {
			foreach (Tank tank in tanks) {
				Destroy(tank.gameObject);
			}
			tanks.Clear();
		}



		void OnDestroy() {
			if (instance == this) {
				instance = null;
			}

			ClearBoxes();
			ClearTanks();
		}

		private List<List<Box>> boxes = new List<List<Box>>();

		private List<Tank> tanks = new List<Tank>();

		private void ShuffleList<T>(List<T> list){
			int index = list.Count;
			while (0 != index) {
				int randomIndex = Random.Range(0, index);
				index--;
				T temp = list[index];
				list[index] = list[randomIndex];
				list[randomIndex] = temp;
			}
		}

	}

}