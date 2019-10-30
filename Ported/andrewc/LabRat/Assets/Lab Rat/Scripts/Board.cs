using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace ECSExamples {

[ExecuteInEditMode]
public class Board : MonoBehaviour {
	public Mesh cellMesh;
	public Material cellMaterial;
	public BoardDesc boardDesc = new BoardDesc(new Vector2Int(10, 10), new Vector2(1f, 1f));
	public GameObject cellPrefab;
	public bool AutoUpdate = true;
	public int TargetFrameRate = -1;

	BoardDesc lastDesc;
	Cell[] cells;

	[System.Serializable]
	public struct BoardDesc {
		public BoardDesc(Vector2Int size, Vector2 cellSize) {
			this.size = size;
			this.cellSize = cellSize;
			this.yNoise = 0f;
			this.colors = new Color[] { Color.white, Color.white };
			this.BorderWalls = true;
		}

		public Vector2Int size;

        [HideInInspector] public Vector2 cellSize;
		public float yNoise;
		public Color[] colors;
		public bool BorderWalls;

		public BoardDesc Copy() {
			BoardDesc copy = this;

			copy.colors = new Color[colors.Length];
			System.Array.Copy(colors, copy.colors, colors.Length);

			return copy;
		}

		public override bool Equals(object obj) {
			if (!(obj is BoardDesc))
				return false;
			var other = (BoardDesc) obj;
			return size == other.size &&
				cellSize == other.cellSize &&
				Mathf.Approximately(yNoise, other.yNoise) &&
				colors.SequenceEqual(other.colors);
		}

		public override int GetHashCode() {
			throw new System.NotImplementedException();
		}

		public override string ToString() {
			return string.Format("<BoardDesc {0}x{1}>", size.x, size.y);
		}
	}

	void OnEnable() {
		cellMesh = Util.CreatePrimitiveMesh(PrimitiveType.Cube);
		if (AutoUpdate)
			Update();
	}

	IEnumerable<Vector2Int> Coords() {
		for (int z = 0; z < boardDesc.size.y; ++z)
			for (int x = 0; x < boardDesc.size.x; ++x)
				yield return new Vector2Int(x, z);
	}

	void Generate() {
		Debug.Log("generating board " + boardDesc);
		var props = new MaterialPropertyBlock();
		int _ColorID = Shader.PropertyToID("_Color");

		System.Array.Resize(ref cells, boardDesc.size.x * boardDesc.size.y);

		int layer = LayerMask.NameToLayer("Board");

		gameObject.layer = layer;

		Util.DestroyChildren(transform);
		foreach (var coord in Coords()) {
			var obj = Instantiate<GameObject>(cellPrefab);
			obj.layer = layer;
			obj.name = "board_" + coord;

			obj.layer = gameObject.layer;
			obj.transform.SetParent(transform);

			// Position the block
			obj.transform.localPosition = new Vector3(
				coord.x * boardDesc.cellSize.x,
				Random.value * boardDesc.yNoise,
				coord.y * boardDesc.cellSize.y);
			
			var index = (coord.x + coord.y) % 2 == 0 ? 1 : 0;
			//var index = (int)Mathf.Repeat((float)count, boardDesc.colors.Length);

			var baseColor = boardDesc.colors[index];

			props.SetColor(_ColorID, boardDesc.colors[index]);
			obj.GetComponent<Renderer>().SetPropertyBlock(props);
			var cell = cells[CoordToIndex(coord)] = obj.GetComponent<Cell>().Init(this, coord, baseColor);
			if (boardDesc.BorderWalls) {
				cell.SetWall(Direction.North, coord.y == boardDesc.size.y - 1);
				cell.SetWall(Direction.East, coord.x == boardDesc.size.x - 1);
				cell.SetWall(Direction.South, coord.y == 0);
				cell.SetWall(Direction.West, coord.x == 0);
			}
		}
		
		lastDesc = boardDesc.Copy();
	}

	int CoordToIndex(Vector2Int coord) {
		return coord.y * boardDesc.size.x + coord.x;
	}

	public void Update () {
		if (AutoUpdate && !lastDesc.Equals(boardDesc))
			Generate();
		
		if (Time.frameCount == lastFrame)
			return;
		
		lastFrame = Time.frameCount;

		if (Application.isPlaying) {
			Application.targetFrameRate = TargetFrameRate;
		}
	}

	static int lastFrame = -1;

    public Vector3 CoordToWorld(Vector2Int cellCoord) {
        return CellAtCoord(cellCoord).transform.position;
    }

	public Cell CellAtWorldPosition(Vector3 worldPosition) {
		var localPt3D = transform.InverseTransformPoint(worldPosition);
		var localPt = new Vector2(localPt3D.x, localPt3D.z);

		localPt += boardDesc.cellSize * 0.5f; // offset by half cellsize
		var cellCoord = new Vector2Int(Mathf.FloorToInt(localPt.x / boardDesc.cellSize.x), Mathf.FloorToInt(localPt.y / boardDesc.cellSize.y));
		return CellAtCoord(cellCoord);
	}

	public Cell RaycastCellDirection(Vector2 screenPos, out Direction cellDirection) {
		cellDirection = Direction.North;

		var ray = Camera.main.ScreenPointToRay(screenPos);

        float enter;

        var plane = new Plane(Vector3.up, new Vector3(0, boardDesc.cellSize.y * 0.5f, 0));

        if (!plane.Raycast(ray, out enter))
			return null;

        var worldPos = ray.GetPoint(enter);
        var cell = CellAtWorldPosition(worldPos);

		if (!cell)
			return null;

		var pt = cell.transform.InverseTransformPoint(worldPos);

		if (Mathf.Abs(pt.z) > Mathf.Abs(pt.x))
			cellDirection = pt.z > 0 ? Direction.North : Direction.South;
		else
			cellDirection = pt.x > 0 ? Direction.East : Direction.West;

		return cell;

	}

    public void RemoveCell(Vector2Int cellCoord) {
        var index = CoordToIndex(cellCoord);
        if (!cells[index])
            return;
        Destroy(cells[index].gameObject);
        cells[index] = null;
    }

	public Cell CellAtCoord(Vector2Int cellCoord) {
		if (cellCoord.x < 0 || cellCoord.y < 0 || cellCoord.x >= boardDesc.size.x || cellCoord.y >= boardDesc.size.y)
			return null;

		return cells[CoordToIndex(cellCoord)];
	}
} 
}
