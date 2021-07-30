using System;
using DOTSRATS;
using Unity.Mathematics;
using UnityEngine;
using UnityEditor;
using Color = UnityEngine.Color;

[CustomEditor(typeof(SceneDebug))]
public class SceneDebugExample : Editor
{
    GUIStyle style;
    void OnEnable()
    {
        if (style == null)
        {
            style = new GUIStyle();
            style.normal.textColor = Color.black;
            style.wordWrap = true;
        }
    }
    
    void OnSceneGUI()
    {
        var dbg = (SceneDebug)target;
        var gameState = dbg.GameState;
        var boardSpawner = dbg.BoardSpawner;

        int2 tileCoord = new int2((int)(dbg.gameObject.transform.position.x + .5f), (int)(dbg.gameObject.transform.position.z + .5f));
        string dbgStr = $"Tile ({tileCoord.x}, {tileCoord.y}):\n";
        if (tileCoord.x < 0 ||
            tileCoord.y < 0 ||
            tileCoord.x >= boardSpawner.boardSize ||
            tileCoord.y >= boardSpawner.boardSize)
        {
            dbgStr += "Invalid coordinate";
        }
        else
        {
            var cell = dbg.CellStructs[tileCoord.y * boardSpawner.boardSize + tileCoord.x];

            dbgStr += $"Walls:";
            foreach (Direction dir in Enum.GetValues(typeof(Direction)))
                if ((cell.wallLayout & dir) != Direction.None)
                    dbgStr += ToString(dir);
                else
                    dbgStr += " ";
            dbgStr += "\n";

            if (cell.arrow != Direction.None)
                dbgStr += $"Arrow: {ToArrowString(cell.arrow)}";

            if (cell.goal)
                dbgStr += "Goal";

            if (cell.hole)
                dbgStr += "Hole";
        }

        Handles.Label(dbg.gameObject.transform.position, dbgStr, style);
    }

    string ToString(Direction dir)
    {
        switch (dir)
        {
            case Direction.None: return "X";
            case Direction.North: return "N";
            case Direction.South: return "S";
            case Direction.East: return "E";
            case Direction.West: return "W";
            case Direction.Up: return "U";
            case Direction.Down: return "D";
            default:
                throw new ArgumentOutOfRangeException(nameof(dir), dir, null);
        }
    }

    string ToArrowString(Direction dir)
    {
        switch (dir)
        {
            case Direction.None: return " ";
            case Direction.North: return "↑";
            case Direction.South: return "↓";
            case Direction.East: return "→";
            case Direction.West: return "←";
            default:
                throw new ArgumentOutOfRangeException(nameof(dir), dir, null);
        }
    }
}
