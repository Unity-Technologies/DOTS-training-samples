using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ECSExamples {

[RequireComponent(typeof(PlayerCursor))]
public class CPUPlayerCursor : MonoBehaviour {
	public int PlayerIndex = -1;
	public Board board;
	public float smoothTime = 0.5f;
	public float maxSpeed = 1f;

    PlayerCursor playerCursor;

	static List<int> FreeIndices;

	Vector2 currentScreenPos;
	Vector2 targetScreenPos;
	Vector2 currentVelocity;

	void OnEnable() {
        playerCursor = GetComponent<PlayerCursor>();

		if (FreeIndices == null)
			FreeIndices = new List<int>() { 1, 2, 3 };

		PlayerIndex = FreeIndices[0];
		FreeIndices.RemoveAt(0);

		if (!board)
			board = FindObjectOfType<Board>();
	}

	void OnDisable() {
		FreeIndices.Add(PlayerIndex);
	}
    
    Homebase homebase;

	void Start() {
        playerCursor.SetColor(PlayerCursor.PlayerColors[PlayerIndex]);
		coro = StartCoroutine(Play());
        homebase = Homebase.ForPlayerIndex(PlayerIndex);
	}

    public void Stop() {
        if (coro != null) {
            done = true;
            StopCoroutine(coro);
            coro = null;
        }
    }

    Coroutine coro;

	void Update() {
		currentScreenPos = Vector2.SmoothDamp(
			currentScreenPos, targetScreenPos, ref currentVelocity,
            smoothTime, maxSpeed, Time.deltaTime);

		playerCursor.SetScreenPosition(currentScreenPos);
	}

    static WaitForSeconds RandomDelay(float minSeconds, float maxSeconds) {
        return new WaitForSeconds(Random.Range(minSeconds, maxSeconds));
    }

    bool done;

	IEnumerator Play() {
        done = false;

		while (!done) {
			var size = board.boardDesc.size;

            Cell cell;
            if (Cell.CellsBeingWalkedOn.Count > 0 && Random.value > 0.2f)
                cell = Cell.CellsBeingWalkedOn.ElementAt(Random.Range(0, Cell.CellsBeingWalkedOn.Count));
            else
                cell = board.CellAtCoord(new Vector2Int(Random.Range(0, size.x), Random.Range(0, size.y)));

			var screenPt = Camera.main.WorldToScreenPoint(cell.GetSurfaceCenter());
			targetScreenPos = screenPt;

			yield return RandomDelay(1.0f, 2.0f);

			if (cell.IsEmpty()) {
                playerCursor.VisualClick();
                
                var toHomebase = homebase.transform.position - cell.transform.position;
                toHomebase.y = 0;

                var dirToHomebase = NearestDirection(toHomebase.normalized);

                //var dir = Cell.RandomDirection();
                var dir = dirToHomebase;

				if (cell.ToggleArrow(dir, PlayerIndex))
					playerCursor.AddAndExpireArrows(cell);
            }

            yield return RandomDelay(0.2f, 0.5f);
		}

        coro = null;
	}

    static Direction NearestDirection(Vector3 v) {
        if (Mathf.Abs(v.x) > Mathf.Abs(v.z))
            return v.x > 0 ? Direction.East : Direction.West;
        else
            return v.z > 0 ? Direction.North : Direction.South;
    }
}

}
