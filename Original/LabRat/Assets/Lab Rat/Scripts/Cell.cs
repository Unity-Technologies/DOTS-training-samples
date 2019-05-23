using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace ECSExamples {

public enum Direction {
	North,
	East,
	South,
	West
}

[ExecuteInEditMode]
public class Cell : MonoBehaviour {
	public Board board;
	public Vector2Int coord;
	public Renderer OverlayRenderer;
    public Renderer OverlayColorRenderer;
    public Color overlayColor = Color.white;

	public GameObject WallPrefab;
    public GameObject HomebasePrefab;

	public GameObject[] Walls = new GameObject[4];

    public Homebase Homebase;

	public Material ArrowMaterial;
	public Material ConfuseMaterial;

    public float LastWalkTime = Mathf.NegativeInfinity;

    public enum ArrowStrength {
        Big,
        Small
    }

    public static HashSet<Cell> CellsBeingWalkedOn = new HashSet<Cell>();

    const string MouseTag = "Mouse";

	public static Direction RandomDirection() {
		return (Direction)Random.Range(0, 4);
	}

	public enum BlockState {
		None,
		Arrow,
		Confuse
	}

	public BlockState blockState = BlockState.None;
	public bool HasArrow {
		get {
			return blockState == BlockState.Arrow;
		}
		set {
			blockState = value ? BlockState.Arrow : BlockState.None;
		}
	}

	public Color ArrowColor;
    public ArrowStrength arrowStrength;
    public int ArrowPlayerIndex = -1;
	public Direction ArrowDirection;

	Renderer _renderer;
	static int _ColorID;

	static MaterialPropertyBlock props;
	static MaterialPropertyBlock block;

	void OnEnable()
	{
		if (props == null)
            props = new MaterialPropertyBlock();

		if (_ColorID == 0)
			_ColorID = Shader.PropertyToID("_Color");

		_renderer = GetComponent<Renderer>();
		UpdateOverlayGraphic();
	}

	public Cell Neighbor(Direction dir) {
		return board.CellAtCoord(coord + WallDirections[(int)dir]);
	}

    // Set the block color
	public void SetColor(Color color) {
		props.SetColor(_ColorID, color);
		_renderer.SetPropertyBlock(props);
	}

	public Cell Init(Board board, Vector2Int coord, Color baseColor) {
		this.board = board;
		this.coord = coord;
		//this.baseColor = baseColor;
		return this;
	}

	static Vector2Int[] WallDirections = new Vector2Int[] {
		Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left
	};

	public Vector3 GetSize() {
		return GetComponent<BoxCollider>().size;
	}

	public Vector3 GetSurfaceCenter() {
		return transform.position + new Vector3(0, GetSize().y / 2f, 0);
	}

    public void SetHomebase(int playerIndex) {
        if (playerIndex == -1 && Homebase) {
            Destroy(Homebase);
        } else if (playerIndex >= 0 && !Homebase) {
            Homebase = Instantiate<GameObject>(HomebasePrefab).GetComponent<Homebase>();
            Assert.IsNotNull(Homebase);
            Homebase.transform.SetParent(transform, false);
            Homebase.transform.localPosition = Vector3.zero;
            Homebase.SetPlayerIndex(playerIndex);
        }
    }

    public bool IsEmpty() {
        return !HasArrow && Homebase == null && blockState == BlockState.None;
    }

	public void SetWall(Direction wall, bool enabled) {
		var idx = (int)wall;

		if (!!Walls[idx] == enabled)
			return;
		
		if (enabled) {
			var mySize = GetSize();
			var obj = Instantiate(WallPrefab, Vector3.zero, Quaternion.identity, transform);
			var y = obj.transform.localScale.y * 0.5f + mySize.y * 0.5f;

			float z = 0;
			float x = 0;
			if (wall == Direction.North || wall == Direction.South) {
				obj.transform.Rotate(0, 90f, 0);
				z = (wall == Direction.North ? 1f : -1f) * (mySize.z * 0.5f - obj.transform.localScale.x);
			} else {
				x = (wall == Direction.East ? 1f : -1f) * (mySize.x * 0.5f - obj.transform.localScale.x);
			}
			obj.transform.localPosition = new Vector3(x, y, z);
			Walls[idx] = obj;
		} else {
			Util.Destroy(Walls[idx]);
			Walls[idx] = null;
			return;
		}
	}

	public static Direction OppositeDirection(Direction dir) {
		return DirectionWhenHitWall(dir, 2);
	}

	static Direction DirectionWhenHitWall(Direction dir, int delta = 1) {
		return (Direction)(((int)dir + delta) % 4);
	}

	public bool HasWall(Direction wall) {
		return !!Walls[(int)wall];
	}

    public int WallOrNeighborWallCount {
        get {
            return (HasWallOrNeighborWall(Direction.North) ? 1 : 0) +
                (HasWallOrNeighborWall(Direction.East) ? 1 : 0) +
                (HasWallOrNeighborWall(Direction.South) ? 1 : 0) +
                (HasWallOrNeighborWall(Direction.West) ? 1 : 0);
        }
    }


	public bool HasWallOrNeighborWall(Direction dir) {
		if (HasWall(dir)) {
			//Debug.Log("cell " + this + " has wall to the " + dir);
			return true;
		}
		
		var neighbor = Neighbor(dir);
		var oppositeDirection = OppositeDirection(dir);
		if (neighbor != null && neighbor.HasWall(oppositeDirection)) {
			//Debug.Log("cell " + neighbor + " has wall to the " + oppositeDirection + " (opposite)");
			return true;
		}

		return false;
	}

	public bool GetArrowDirection(out Direction arrowDirection) {
		arrowDirection = ArrowDirection;
		return HasArrow;
	}

    public bool AbsorbsWalker(Walks walker) {
        if (Homebase) {
            Homebase.DidAbsorb(walker);
            return true;
        }

        return false;
    }

	public Direction ShouldRedirect(Direction myDirection, ref Vector2Int lastRedirectCoord, Walks walker) {
		if (blockState == BlockState.Confuse) {
			const int numDirections = 4;
			var nextIndex = ((int)myDirection + 1 + Random.Range(0, numDirections - 1)) % numDirections;
			myDirection = (Direction)nextIndex;
		}

		Direction arrowDirection;
		bool hasArrow = GetArrowDirection(out arrowDirection);
		if (hasArrow && myDirection != arrowDirection) {
			if (arrowDirection == OppositeDirection(myDirection) && walker.DiminishesArrows)
				DiminishArrow();

			myDirection = arrowDirection;
		}
			
		if (HasWallOrNeighborWall(myDirection)) {
			myDirection = DirectionWhenHitWall(myDirection);
			if (HasWallOrNeighborWall(myDirection)) {
				myDirection = OppositeDirection(myDirection);
				if (HasWallOrNeighborWall(myDirection)) {
					myDirection = OppositeDirection(myDirection);
				}
			}
		}

		return myDirection;
	}

    void DiminishArrow() {
        if (!HasArrow) {
            return;
        } else if (arrowStrength == ArrowStrength.Big) {
            arrowStrength = ArrowStrength.Small;
            UpdateOverlayGraphic();
        } else {
            ToggleArrow(ArrowDirection);

            // notify players that their arrow was gone
            PlayerCursor.ArrowWasRemoved(this);
        }
    }

	public void SetArrow(Direction dir, ArrowStrength strength, int playerIndex) {
		HasArrow = true;
		ArrowDirection = dir;
		ArrowColor = new Color(1f, 1f, 1f, 1f);
        arrowStrength = ArrowStrength.Big;
		overlayColor = PlayerCursor.PlayerColors[playerIndex];
        ArrowPlayerIndex = playerIndex;
		UpdateOverlayGraphic();
	}

    public bool CanChangeArrow(int playerIndex) {
        return !HasArrow || ArrowPlayerIndex == playerIndex;
    }

	public bool ToggleArrow(Direction dir, int playerIndex = 0) {
		HasArrow = !HasArrow;
		ArrowDirection = dir;
        arrowStrength = ArrowStrength.Big;
		ArrowColor = new Color(1f, 1f, 1f, HasArrow ? 1f : 0f);
        overlayColor = PlayerCursor.PlayerColors[playerIndex];
        ArrowPlayerIndex = playerIndex;
		UpdateOverlayGraphic();
		return HasArrow;
	}

	public void RemoveArrow() {
		HasArrow = false;
		ArrowColor = new Color(1f, 1f, 1f, 0f);
        ArrowPlayerIndex = -1;
		UpdateOverlayGraphic();
	}

    void Update() {
        if (LastWalkTime < 0)
            return;

        var walkSince = Time.time - LastWalkTime;
        if (walkSince > 2.0f)
            CellsBeingWalkedOn.Remove(this);
        else
            CellsBeingWalkedOn.Add(this);
    }

	void UpdateOverlayGraphic() {
		if (OverlayRenderer == null)
			return;

		float z;
		Material mat;
		Color color;
        Vector3 localScale;

		if (blockState == BlockState.Confuse) {
			z = 0f;
			mat = ConfuseMaterial;
			color = Color.white;
            localScale = Vector3.one;
		} else {
			z = ((int)ArrowDirection) * -90f;
			mat = ArrowMaterial;
			color = ArrowColor;
            localScale = arrowStrength == ArrowStrength.Small ? new Vector3(0.55f, 0.55f, 0.55f) : new Vector3(0.85f, 0.85f, 0.85f);
		}

		OverlayRenderer.transform.localRotation = Quaternion.Euler(90f, 0, z);
        OverlayRenderer.transform.localScale = localScale;
		OverlayRenderer.sharedMaterial = mat;
        bool hasOverlay = color.a > 0f || blockState == BlockState.Confuse;

		OverlayRenderer.gameObject.SetActive(hasOverlay);
        OverlayColorRenderer.enabled = hasOverlay && HasArrow;

		props.SetColor(_ColorID, color);
		OverlayRenderer.SetPropertyBlock(props);

        props.SetColor(_ColorID, overlayColor);
        OverlayColorRenderer.SetPropertyBlock(props);
	}

	public void ToggleWall(Direction dir) {
		SetWall(dir, !HasWall(dir));
	}

	public void ToggleConfuse() {
		blockState = blockState == BlockState.Confuse ? BlockState.None : BlockState.Confuse;
		UpdateOverlayGraphic();
	}

    public void ToggleHomebase() {
        SetHomebase(Homebase == null ? 0 : -1);
    }

	public string ToCell() {
		return "<Cell " + coord + ">";
	}

	internal void _OnMouseOver(Direction dir) {
		if (!HasArrow) {
			ArrowDirection = dir;
			ArrowColor = new Color(1f, 1f, 1f, 0.5f);
			UpdateOverlayGraphic();
		}
	}

	internal void _OnMouseExit() {
		if (!HasArrow) {
			ArrowColor = new Color(1f, 1f, 1f, 0f);
			UpdateOverlayGraphic();
		}
	}


}

}
