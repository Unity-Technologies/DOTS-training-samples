using System;
using ECSExamples;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public struct PlayerInput : IBufferElementData
{
    // TODO: duplicate data between screen pos and cell coord
    public float3 ScreenPosition;
    public int2 CellCoordinates;
    public Direction Direction;
    public bool Clicked;
}

[UpdateBefore(typeof(WalkSystem))]
public class ArrowSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        var board = GetSingleton<BoardDataComponent>();
        var cellMap = World.GetExistingSystem<BoardSystem>().CellMap;
        var arrowMap = World.GetExistingSystem<BoardSystem>().ArrowMap;

        // Draws a cursor with the player color on top of the default one
        //playerCursor.SetScreenPosition(screenPos);

        var overlayEntities = GetEntityQuery(typeof(OverlayComponentTag), typeof(Translation)).ToEntityArray(Allocator.TempJob);
        var overlayPositions = GetEntityQuery(typeof(OverlayComponentTag), typeof(Translation)).ToComponentDataArray<Translation>(Allocator.TempJob);
        var overlayPlacementTick = GetEntityQuery(typeof(OverlayComponentTag), typeof(OverlayPlacementTickComponent))
            .ToComponentDataArray<OverlayPlacementTickComponent>(Allocator.TempJob);
        var overlayColorEntities = GetEntityQuery(typeof(OverlayColorComponent), typeof(Translation)).ToEntityArray(Allocator.TempJob);
        Entities.ForEach((Entity entity, DynamicBuffer<PlayerInput> inputBuffer, ref PlayerComponent player) =>
        {
            PlayerInput input = default;
            if (inputBuffer.Length == 0)
                return;
            // TODO: Mimic command input handling
            input = inputBuffer[inputBuffer.Length - 1];

            if (input.Clicked && HasSingleton<GameInProgressComponent>())
            {
                var cellIndex = input.CellCoordinates.y * board.size.y + input.CellCoordinates.x;

                // Update the cell data and arrow map with this arrow placement, used by game logic
                // TODO: Handle the case for replacing existing arrow
                CellComponent cell = new CellComponent();
                if (cellMap.ContainsKey(cellIndex))
                {
                    cell = cellMap[cellIndex];
                    // Deny placing arrow on top of bases or holes (thin clients do this)
                    if ((cell.data & CellData.HomeBase) == CellData.HomeBase ||
                        (cell.data & CellData.Hole) == CellData.Hole)
                        return;
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
                    Coordinates = input.CellCoordinates,
                    Direction = input.Direction,
                    PlayerId = player.PlayerId,
                    PlacementTick = Time.time
                };

                int arrowCount = 0;
                int oldestIndex = -1;
                float oldestTick = float.MaxValue;
                var keys = arrowMap.GetKeyArray(Allocator.TempJob);
                for (int i = 0; i < keys.Length; ++i)
                {
                    if (arrowMap[keys[i]].PlayerId == player.PlayerId)
                    {
                        // Bail if it's an identical placement as has been done before
                        if (arrowMap[keys[i]].PlayerId == player.PlayerId
                            && arrowMap[keys[i]].Coordinates.x == input.CellCoordinates.x
                            && arrowMap[keys[i]].Coordinates.y == input.CellCoordinates.y
                            && arrowMap[keys[i]].Direction == input.Direction)
                            return;

                        arrowCount++;
                        if (arrowMap[keys[i]].PlacementTick < oldestTick)
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
                    if (cell.data == 0)
                        cellMap.Remove(oldestIndex);
                    else
                        cellMap[oldestIndex] = cell;
                }

                if (arrowMap.ContainsKey(cellIndex))
                    arrowMap[cellIndex] = arrowData;
                else
                    arrowMap.Add(cellIndex, arrowData);
                Debug.Log("Placed arrow on cell="+input.CellCoordinates + " with dir=" + input.Direction);

                // Update the overlays (arrow+color) which shows the player visually where the arrow is placed
                var startIndex = (player.PlayerId-1) * PlayerConstants.MaxArrows;
                oldestTick = float.MaxValue;
                oldestIndex = -1;
                for (int i = startIndex; i < (startIndex+PlayerConstants.MaxArrows); ++i)
                {
                    if (overlayPositions[i].Value.y >= 9.9f)
                    {
                        Debug.Log("Setting overlay " + overlayColorEntities[i] + " to player " + player.PlayerId);
                        oldestIndex = i;
                        break;
                    }
                    if (oldestTick > overlayPlacementTick[i].Tick)
                    {
                        oldestIndex = i;
                        oldestTick = overlayPlacementTick[i].Tick;
                    }
                }

                Debug.Log("Player " + player.PlayerId + " now owns index=" + oldestIndex + " coord=" + input.CellCoordinates);
                PostUpdateCommands.SetComponent(overlayEntities[oldestIndex], new OverlayPlacementTickComponent { Tick = Time.time});
                PostUpdateCommands.SetComponent(overlayEntities[oldestIndex], new Translation { Value = new float3(input.CellCoordinates.x,0.7f,input.CellCoordinates.y)});
                var rotation = quaternion.RotateX(math.PI / 2);
                switch (input.Direction) {
                    case Direction.South:
                        rotation = math.mul(rotation, quaternion.RotateZ(math.PI));
                        break;
                    case Direction.East:
                        rotation = math.mul(rotation, quaternion.RotateZ(3*math.PI/2));
                        break;
                    case Direction.West:
                        rotation = math.mul(rotation, quaternion.RotateZ(math.PI/2));
                        break;
                }
                PostUpdateCommands.SetComponent(overlayEntities[oldestIndex], new Rotation { Value = rotation});
                PostUpdateCommands.SetComponent(overlayColorEntities[oldestIndex], new Translation { Value = new float3(input.CellCoordinates.x,0.6f,input.CellCoordinates.y)});
            }

            // Update the current position of the players cursor
            PostUpdateCommands.SetComponent(entity, new Translation{Value = input.ScreenPosition});
        });
        overlayPlacementTick.Dispose();
        overlayColorEntities.Dispose();
        overlayPositions.Dispose();
        overlayEntities.Dispose();
    }
}

public class ClientArrowSystem : ComponentSystem
{
    //private EndSimulationEntityCommandBufferSystem m_Buffer;

    protected override void OnCreate()
    {
        //m_Buffer = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var board = GetSingleton<BoardDataComponent>();

        /*var time = Time.time;
        var deltaTime = Time.deltaTime;
        var ecb = m_Buffer.CreateCommandBuffer().ToConcurrent();
        var aiInputs = GetBufferFromEntity<PlayerInput>();
        var job = Entities.ForEach((Entity playerEntity, int entityInQueryIndex, ref AiPlayerComponent arrowData, ref PlayerComponent player) =>
        {
            float2 aiCellCoord = float2.zero;
            Direction aiCellDirection = Direction.North;
            bool aiCellClicked = false;
            float3 aiScreenPos = float3.zero;

            var playerId = player.PlayerId + 1;
            var random = new Random();
            random.InitState((uint)((time+1)*10000 * playerId));

            var shouldClick = random.NextFloat(0, 1);
            var currentPosition = arrowData.CurrentPosition;
            if (arrowData.StartTime > 0f && Math.Abs(currentPosition.x - arrowData.TargetPosition.x) < 0.1f &&
                Math.Abs(currentPosition.y - arrowData.TargetPosition.y) < 0.1f)
            {
                aiCellClicked = true;
                aiCellDirection = arrowData.Direction;
                aiCellCoord = arrowData.CellCoordinate;
                arrowData.StartTime = 0f;
            }
            else if (arrowData.StartTime + 2f < time && shouldClick > 0.95f)
            {
                Helpers.GetRandomArrowPlacement(out var nextPosition, out var nextDirection, out aiCellCoord, board, playerId, random);
                arrowData.TargetPosition = Camera.main.WorldToScreenPoint(nextPosition);
                arrowData.Direction = nextDirection;
                arrowData.CellCoordinate = aiCellCoord;
                arrowData.StartTime = time;
                ecb.SetComponent(entityInQueryIndex, playerEntity, arrowData);
            }

            if (arrowData.TargetPosition.x == 0 && arrowData.TargetPosition.z == 0)
                return;

            // Gradually move towards the position of the next arrow placement
            Vector2 currentVelocity = Vector2.zero;
            var currentScreenPos = Vector2.SmoothDamp(
                new Vector2(currentPosition.x, currentPosition.y), new Vector2(arrowData.TargetPosition.x, arrowData.TargetPosition.y), ref currentVelocity,
                0.01f, 400f, deltaTime);
            arrowData.CurrentPosition = new float3(currentScreenPos.x, currentScreenPos.y, 0f);
            aiScreenPos = arrowData.CurrentPosition;
            ecb.SetComponent(entityInQueryIndex, playerEntity, arrowData);

            aiInputs[playerEntity].Add(new PlayerInput
            {
                ScreenPosition = aiScreenPos,
                Direction = aiCellDirection,
                CellCoordinates = (int2)aiCellCoord,
                Clicked = aiCellClicked
            });
        }).WithReadOnly(aiInputs).Schedule(inputDeps);*/

        float2 cellCoord = float2.zero;
        Direction cellDirection = Direction.North;
        bool cellClicked = false;
        float3 screenPos = float3.zero;
        {
            // Input gathering
            cellClicked = Input.GetMouseButtonDown(0);
            screenPos = Input.mousePosition;
            int cellIndex;
            float3 worldPos;
            Helpers.ScreenPositionToCell(screenPos, board.cellSize, board.size, out worldPos, out cellCoord, out cellIndex);

            var localPos = new float2(worldPos.x - cellCoord.x, worldPos.z - cellCoord.y);
            if (Mathf.Abs(localPos.y) > Mathf.Abs(localPos.x))
                cellDirection = localPos.y > 0 ? Direction.North : Direction.South;
            else
                cellDirection = localPos.x > 0 ? Direction.East : Direction.West;

            // Show arrow placement overlay (before it's placed permanently)
            var hoverArrowComponent = EntityManager.CreateEntity();
            EntityManager.AddComponentData(hoverArrowComponent, new ArrowComponent
            {
                Coordinates = cellCoord,
                Direction = cellDirection,
                PlacementTick = Time.time
            });
        }

        if (!HasSingleton<LocalPlayerComponent>())
            return;
        var localInput = GetSingletonEntity<LocalPlayerComponent>();

        var input = default(PlayerInput);
        input.ScreenPosition = screenPos;
        input.Direction = cellDirection;
        input.CellCoordinates = (int2)cellCoord;
        input.Clicked = cellClicked;
        var inputBuffer = EntityManager.GetBuffer<PlayerInput>(localInput);
        inputBuffer.Add(input);
    }
}

public class Helpers
{
    public static void ScreenPositionToCell(float3 position, float2 cellSize, int2 boardSize, out float3 worldPos, out float2 cellCoord, out int cellIndex)
    {
        cellCoord = new float2(0f,0f);
        cellIndex = 0;
        worldPos = float3.zero;;

        var ray = Camera.main.ScreenPointToRay(position);
        float enter;
        var plane = new Plane(math.up(), new float3(0, cellSize.y * 0.5f, 0));
        if (!plane.Raycast(ray, out enter))
            return;

        worldPos = ray.GetPoint(enter);
        if (worldPos.x < 0 || Mathf.FloorToInt(worldPos.x) >= boardSize.x-1 || worldPos.z < 0 || Mathf.FloorToInt(worldPos.z) >= boardSize.y-1)
            return;
        var localPt = new float2(worldPos.x, worldPos.z);
        localPt += cellSize * 0.5f; // offset by half cellsize
        cellCoord = new float2(Mathf.FloorToInt(localPt.x / cellSize.x), Mathf.FloorToInt(localPt.y / cellSize.y));
        cellIndex = (int)(cellCoord.y * boardSize.x + cellCoord.x);
    }

    public static void GetRandomArrowPlacement(out float3 nextPosition, out Direction cellDirection, out float2 cellCoord, BoardDataComponent board, int playerId, Random random)
    {
        var nextX = random.NextInt(0, board.size.x);
        var nextY = random.NextInt(0, board.size.y);
        nextPosition = new float3(nextX, 0.55f, nextY);
        var localPt = new float2(nextPosition.x, nextPosition.z);
        localPt += board.cellSize * 0.5f;
        cellCoord = new float2(Mathf.FloorToInt(localPt.x / board.cellSize.x), Mathf.FloorToInt(localPt.y / board.cellSize.y));

        var directionValue = random.NextInt(0, 100);
        if (directionValue < 25)
            cellDirection = Direction.East;
        else if (directionValue < 50)
            cellDirection = Direction.West;
        else if (directionValue < 75)
            cellDirection = Direction.South;
        else
            cellDirection = Direction.North;
    }
}
