using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ECSExamples {

public class CursorFollowMouse : MonoBehaviour {
    PlayerCursor playerCursor;
    Cell lastCell;
    public Board board;

    void Start () {
        playerCursor = GetComponent<PlayerCursor>();
        playerCursor.SetColor(PlayerCursor.PlayerColors[0]);
    }

    void _ToggleArrow(Cell cell, Direction dir) {
        if (cell.ToggleArrow(dir)) {
            playerCursor.AddAndExpireArrows(cell);
        } else {
            playerCursor.RemoveArrow(cell);
        }
    }

    void Update() {
        var screenPos = Input.mousePosition;
        playerCursor.SetScreenPosition(screenPos);

        Direction dir;
        var cell = board.RaycastCellDirection(screenPos, out dir);

        if (cell)
            cell._OnMouseOver(dir);
        if (cell != lastCell && lastCell)
            lastCell._OnMouseExit();

        lastCell = cell;

        if (cell == null)
            return;

        const int humanPlayerIndex = 0;

        var canChangeArrow = cell.CanChangeArrow(humanPlayerIndex);

        if (Input.GetMouseButtonDown(0)) {
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                cell.ToggleWall(dir);
            else if (canChangeArrow)
                _ToggleArrow(cell, dir);
        }

        if (Input.GetKeyDown(KeyCode.C))
            cell.ToggleConfuse();

        if (Input.GetKeyDown(KeyCode.H))
            cell.ToggleHomebase();

        if (canChangeArrow) {
            if (Input.GetKeyDown(KeyCode.W))
                _ToggleArrow(cell, Direction.North);
            if (Input.GetKeyDown(KeyCode.D))
                _ToggleArrow(cell, Direction.East);
            if (Input.GetKeyDown(KeyCode.S))
                _ToggleArrow(cell, Direction.South);
            if (Input.GetKeyDown(KeyCode.A))
                _ToggleArrow(cell, Direction.West);
        }
    }
}


}
