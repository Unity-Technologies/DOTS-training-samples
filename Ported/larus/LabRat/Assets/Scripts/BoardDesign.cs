using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
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

        SpawnerAt(MouseSpawner, 0, 0, Quaternion.identity);
        SpawnerAt(MouseSpawner, boardSize.x - 1, boardSize.y - 1, Quaternion.Euler(0, 180, 0));
        SpawnerAt(CatSpawner, 0, boardSize.y - 1, Quaternion.Euler(0, 0, 0));
        SpawnerAt(CatSpawner, boardSize.x - 1, 0, Quaternion.Euler(0, 0, 0));
        if (boardSize.x > 13)
        {
	        float offset = boardSize.x / 4f;
	        SpawnerAt(MouseSpawner, offset, offset, Quaternion.identity);
	        SpawnerAt(MouseSpawner, offset, offset * 3f, Quaternion.Euler(0, 180, 0));
	        SpawnerAt(MouseSpawner, offset * 3f, offset, Quaternion.identity);
	        SpawnerAt(MouseSpawner, offset * 3f, offset * 3f, Quaternion.Euler(0, 180, 0));
	        offset = boardSize.x / 6f;
	        SpawnerAt(MouseSpawner, offset, offset, Quaternion.identity);
	        SpawnerAt(MouseSpawner, offset, offset * 5f, Quaternion.Euler(0, 180, 0));
	        SpawnerAt(MouseSpawner, offset * 5f, offset, Quaternion.identity);
	        SpawnerAt(MouseSpawner, offset * 5f, offset * 5f, Quaternion.Euler(0, 180, 0));
        }
        if (boardSize.x >= 60)
        {
	        float offset = boardSize.x / 2f;
	        SpawnerAt(MouseSpawner, 0, offset, Quaternion.identity);
	        SpawnerAt(MouseSpawner, offset, 0, Quaternion.Euler(0, 180, 0));
	        SpawnerAt(MouseSpawner, boardSize.x - 1, offset, Quaternion.identity);
	        SpawnerAt(MouseSpawner, offset, boardSize.y - 1, Quaternion.Euler(0, 180, 0));
			offset = boardSize.x / 5f;
	        SpawnerAt(MouseSpawner, offset, offset, Quaternion.identity);
	        SpawnerAt(MouseSpawner, offset, offset*4f, Quaternion.Euler(0, 180, 0));
	        SpawnerAt(MouseSpawner, offset*4f, offset, Quaternion.identity);
	        SpawnerAt(MouseSpawner, offset*4f, offset*4f, Quaternion.Euler(0, 180, 0));
	        offset = boardSize.x / 7f;
	        SpawnerAt(MouseSpawner, offset, offset, Quaternion.identity);
	        SpawnerAt(MouseSpawner, offset, offset*6f, Quaternion.Euler(0, 180, 0));
	        SpawnerAt(MouseSpawner, offset*6f, offset, Quaternion.identity);
	        SpawnerAt(MouseSpawner, offset*6f, offset*6f, Quaternion.Euler(0, 180, 0));
        }
        if (boardSize.x >= 100)
        {
	        float offset = boardSize.x * 4f / 10f;
	        SpawnerAt(MouseSpawner, offset, offset, Quaternion.identity);
	        SpawnerAt(MouseSpawner, 0, offset*1.5f, Quaternion.Euler(0, 180, 0));
	        SpawnerAt(MouseSpawner, offset*1.5f,0,  Quaternion.identity);
	        SpawnerAt(MouseSpawner, offset*1.5f, offset*1.5f, Quaternion.Euler(0, 180, 0));
	        offset = boardSize.x / 6f;
	        SpawnerAt(MouseSpawner, 0, offset, Quaternion.identity);
	        SpawnerAt(MouseSpawner, offset, 0, Quaternion.Euler(0, 180, 0));
	        SpawnerAt(MouseSpawner, boardSize.x - 1, offset, Quaternion.identity);
	        SpawnerAt(MouseSpawner, offset*4f, boardSize.y - 1, Quaternion.Euler(0, 180, 0));
	        offset = boardSize.x / 8f;
	        SpawnerAt(MouseSpawner, 0, offset, Quaternion.identity);
	        SpawnerAt(MouseSpawner, offset, 0, Quaternion.Euler(0, 180, 0));
	        SpawnerAt(MouseSpawner, boardSize.x - 1, offset, Quaternion.identity);
	        SpawnerAt(MouseSpawner, offset*4f, boardSize.y - 1, Quaternion.Euler(0, 180, 0));
        }

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
