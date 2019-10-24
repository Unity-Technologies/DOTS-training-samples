using ECSExamples;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
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
        if (worldPos.x < 0 || Mathf.FloorToInt(worldPos.x) >= board.size.x-1 || worldPos.z < 0 || Mathf.FloorToInt(worldPos.z) >= board.size.y-1)
            return;
        var localPt = new float2(worldPos.x, worldPos.z);
        localPt += board.cellSize * 0.5f; // offset by half cellsize
        var cellCoord = new float2(Mathf.FloorToInt(localPt.x / board.cellSize.x), Mathf.FloorToInt(localPt.y / board.cellSize.y));
        var cellIndex = (int)(cellCoord.y * board.size.x + cellCoord.x);

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
        //    (Player ID) - server could figure that out from input sender

        // Show arrow placement overlay (before it's placed permanently)
        var cellEntities = EntityManager.CreateEntityQuery(typeof(CellRenderingComponentTag)).ToEntityArray(Allocator.TempJob);
        for (int i = 0; i < cellEntities.Length; ++i)
        {
            var position = EntityManager.GetComponentData<Translation>(cellEntities[i]);
            localPt = new float2(position.Value.x, position.Value.z) + board.cellSize * 0.5f;
            var rendCellCoord = new float2(Mathf.FloorToInt(localPt.x / board.cellSize.x), Mathf.FloorToInt(localPt.y / board.cellSize.y));
            var rendCellIndex = (int)(rendCellCoord.y * board.size.x + rendCellCoord.x);
            if (rendCellIndex == cellIndex)
                EntityManager.AddComponentData(cellEntities[i], new ArrowComponent
                {
                    Coordinates = cellCoord,
                    Direction = cellDirection,
                    PlayerId = 0,
                    PlacementTick = Time.time
                });
        }
        /*var entity = EntityManager.CreateEntity();
        EntityManager.AddComponentData(entity, new ArrowComponent
        {
            Coordinates = cellCoord,
            Direction = cellDirection,
            PlayerId = 0,
            PlacementTick = Time.frameCount
        });*/

        // Human always player 0
        const int humanPlayerIndex = 0;
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
                PlayerId = humanPlayerIndex,
                PlacementTick = Time.time
            };

            // Add entity for visual representation of arrow
            //entity = EntityManager.CreateEntity();
            //EntityManager.AddComponentData(entity, arrowData);

            // Or use hash map and draw everything in it somewhere
            var keys = arrowMap.GetKeyArray(Allocator.TempJob);
            int arrowCount = 0;
            int oldestIndex = -1;
            float oldestTick = -1;
            for (int i = 0; i < keys.Length; ++i)
            {
                if (arrowMap[keys[i]].PlayerId == humanPlayerIndex)
                {
                    arrowCount++;
                    if (arrowMap[keys[i]].PlacementTick > oldestTick)
                    {
                        oldestIndex = keys[i];
                        oldestTick = arrowMap[keys[i]].PlacementTick;
                    }
                }
            }
            keys.Dispose();
            if (arrowCount >= 3)
            {
                arrowMap.Remove(oldestIndex);
                cell = cellMap[oldestIndex];
                cell.data &= ~CellData.Arrow;
                cellMap[oldestIndex] = cell;
            }

            if (arrowMap.ContainsKey(cellIndex))
                arrowMap[cellIndex] = arrowData;
            else
                arrowMap.Add(cellIndex, arrowData);
            Debug.Log("Placed arrow on cell="+cellCoord + " with dir=" + cellDirection);
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
