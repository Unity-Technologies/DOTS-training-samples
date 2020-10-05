using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ECSExamples {

public class BoardDesign : MonoBehaviour {
	public enum Design {
		SimpleQuadrants,
		RandomWalls,
	}

	public Design design;
	public int RandomSeed = -1;

	public GameObject MouseSpawner;
	public GameObject CatSpawner;
    public GameObject HomebasePrefab;

    Board board;

	void OnEnable () {
		board = GetComponent<Board>();
		board.Update();
		var boardSize = board.boardDesc.size;

		var oldState = Random.state;
        if (RandomSeed != -1)
            Random.InitState(RandomSeed);

		if (design == Design.SimpleQuadrants) {
			const int wallLength = 3;
			for (int y = 0; y < wallLength; ++y) {
                {
                    var cell = board.CellAtCoord(new Vector2Int((int)(boardSize.x / 2f), y));
                    if (cell) cell.SetWall(Direction.East, true);
                }
                {
                    var cell = board.CellAtCoord(new Vector2Int((int)(boardSize.x / 2f), boardSize.y - 1 - y));
                    if (cell) cell.SetWall(Direction.West, true);
                }
			}

			for (int x = 0; x < wallLength; ++x) {
                {
                    var cell = board.CellAtCoord(new Vector2Int(x, ((int)(boardSize.y / 2f))));
                    if (cell) cell.SetWall(Direction.South, true);
                }
                {
                    var cell = board.CellAtCoord(new Vector2Int(boardSize.x - 1 - x, ((int)(boardSize.y / 2f))));
                    if (cell) cell.SetWall(Direction.North, true);
                }
			}
		} else if (design == Design.RandomWalls) {
			int numWalls = (int)(boardSize.x * boardSize.y * 0.2f);
			for (int c = 0; c < numWalls; ++c) {
				var cell = board.CellAtCoord(new Vector2Int(Random.Range(0, boardSize.x), Random.Range(0, boardSize.y)));
                if (!cell) continue;
				var direction = Cell.RandomDirection();
				if (cell.HasWallOrNeighborWall(direction) || cell.WallOrNeighborWallCount > 3)
					c--;
				else
					cell.SetWall(direction, true);
			}
		}

        float offset = 1f / 3f;

        // setup home bases
        CellAt(boardSize.x * offset, boardSize.y * offset).SetHomebase(0);
        CellAt(boardSize.x * 2f * offset, boardSize.y * 2f * offset).SetHomebase(1);
        CellAt(boardSize.x * offset, boardSize.y * 2f * offset).SetHomebase(2);
        CellAt(boardSize.x * 2f * offset, boardSize.y * offset).SetHomebase(3);

        SpawnerAt(MouseSpawner, 0, 0, Quaternion.identity);
        SpawnerAt(MouseSpawner, boardSize.x - 1, boardSize.y - 1, Quaternion.Euler(180, 0, 0));
        SpawnerAt(CatSpawner, 0, boardSize.y - 1, Quaternion.Euler(0, 0, 0));
        SpawnerAt(CatSpawner, boardSize.x - 1, 0, Quaternion.Euler(0, 0, 0));

        int numHoles = Random.Range(0, 4);
        for (int i = 0; i < numHoles; ++i) {
            var coord = new Vector2Int(Random.Range(0, boardSize.x), Random.Range(0, boardSize.y));
            if (coord.x > 0 && coord.y > 0 && coord.x < boardSize.x - 1 && coord.y < boardSize.y - 1 && board.CellAtCoord(coord).IsEmpty())
                board.RemoveCell(coord);
        }

		Random.state = oldState;
	}

    void SpawnerAt(GameObject spawner, float cellX, float cellY, Quaternion rotation) {
        var worldPos = board.CoordToWorld(new Vector2Int((int)cellX, (int)cellY));
		var s = Instantiate<GameObject>(spawner, worldPos, rotation, parent: null).GetComponent<Spawner>();
		s.board = board;
    }

    Cell CellAt(float cellX, float cellY) {
        return board.CellAtCoord(new Vector2Int((int)cellX, (int)cellY));
    }
	
}

}
