using ECSExamples;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public class ArrowSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        var board = GetSingleton<BoardDataComponent>();
        var cellMap = World.GetExistingSystem<BoardSystem>().CellMap;
        var arrowMap = World.GetExistingSystem<BoardSystem>().ArrowMap;

        // INPUT PROCESSING PART
        var screenPos = Input.mousePosition;

        // Draws a cursor with the player color on top of the default one
        //playerCursor.SetScreenPosition(screenPos);

        var ray = Camera.main.ScreenPointToRay(screenPos);
        float enter;
        var plane = new Plane(Vector3.up, new Vector3(0, board.cellSize.y * 0.5f, 0));
        if (!plane.Raycast(ray, out enter))
            return;

        var worldPos = ray.GetPoint(enter);
        var localPt = new float2(worldPos.x, worldPos.z);
        localPt += board.cellSize * 0.5f; // offset by half cellsize
        var cellCoord = new float2(Mathf.FloorToInt(localPt.x / board.cellSize.x), Mathf.FloorToInt(localPt.y / board.cellSize.y));
        var cellIndex = (int)(cellCoord.y * board.size.x + cellCoord.x);
        if (cellIndex < 0 || cellIndex >= board.size.x * board.size.y)
            return;

        Direction cellDirection;
        var localPos = new float2(worldPos.x - cellCoord.x, worldPos.z - cellCoord.y);
        if (Mathf.Abs(localPos.y) > Mathf.Abs(localPos.x))
            cellDirection = localPos.y > 0 ? Direction.North : Direction.South;
        else
            cellDirection = localPos.x > 0 ? Direction.East : Direction.West;
        //Debug.Log("dir=" + cellDirection + " worldPos="+worldPos + " cellPos=" + cellCoord + " localPos=" + localPos);

        // Input data:
        //    ArrowDirection
        //    CellIndex
        //    Player ID

        // Human always player 0
        //const int humanPlayerIndex = 0;
        //var canChangeArrow = cell.CanChangeArrow(humanPlayerIndex);

        // INPUT HANDLING PART:
        if (Input.GetMouseButtonDown(0))
        {
            // Wall building stuff
            //if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                //cell.ToggleWall(dir);

            //_ToggleArrow(cell, dir);
            CellComponent cell = new CellComponent();
            if (cellMap.ContainsKey(cellIndex))
            {
                cell = cellMap[cellIndex];
                cell.data |= CellData.Arrow;
                cellMap[cellIndex] = cell;
            }
            else
            {
                cell.data |= CellData.Arrow;
                cellMap.Add(cellIndex, cell);
            }

            var arrowData = new ArrowComponent
            {
                Coordinates = cellCoord,
                Direction = cellDirection,
                PlayerId = 0
            };

            // TODO: Add entities for arrows then have a system which draws all existing arrow components
            /*var entity = EntityManager.CreateEntity();
            EntityManager.AddComponentData(entity, arrowData);*/

            // Or use hash map and draw everything in it somewhere
            arrowMap.Add(cellIndex, arrowData);
            Debug.Log("Placed arrow on cell="+cellCoord + " with dir=" + cellDirection);

            // TODO: Remove oldest arrow if you have 3 in total now
        }

        /*if (cell)
            cell._OnMouseOver(dir);
        if (cell != lastCell && lastCell)
            lastCell._OnMouseExit();

        lastCell = cell;

        if (cell == null)
            return;*/


        /*if (Input.GetKeyDown(KeyCode.C))
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
        }*/
    }
}
