using System;
using ECSExamples;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Networking.Transport;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public struct PlayerInput : ICommandData<PlayerInput>
{
    // TODO: duplicate data between screen pos and cell coord
    public float3 ScreenPosition;
    public int2 CellCoordinates;
    public Direction Direction;
    public bool Clicked;

    public uint Tick => tick;
    public uint tick;

    public void Serialize(DataStreamWriter writer)
    {
        writer.Write(ScreenPosition.x);
        writer.Write(ScreenPosition.y);
        writer.Write(CellCoordinates.x);
        writer.Write(CellCoordinates.y);
        writer.Write((byte)Direction);
        writer.Write((byte)(Clicked ? 1 : 0));
    }

    public void Deserialize(uint tick, DataStreamReader reader, ref DataStreamReader.Context ctx)
    {
        this.tick = tick;
        var screenX = reader.ReadFloat(ref ctx);
        var screenY = reader.ReadFloat(ref ctx);
        ScreenPosition = new float3(screenX, screenY, 0);
        var cellX = reader.ReadInt(ref ctx);
        var cellY = reader.ReadInt(ref ctx);
        CellCoordinates = new int2(cellX, cellY);
        Direction = (Direction)reader.ReadByte(ref ctx);
        Clicked = reader.ReadByte(ref ctx) != 0;
    }

    public void Serialize(DataStreamWriter writer, PlayerInput baseline, NetworkCompressionModel compressionModel)
    {
        Serialize(writer);
    }

    public void Deserialize(uint tick, DataStreamReader reader, ref DataStreamReader.Context ctx, PlayerInput baseline,
        NetworkCompressionModel compressionModel)
    {
        Deserialize(tick, reader, ref ctx);
    }
}

[UpdateInGroup(typeof(ServerSimulationSystemGroup))]
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
            var simulationSystemGroup = World.GetExistingSystem<ServerSimulationSystemGroup>();
            var inputTick = simulationSystemGroup.ServerTick;
            PlayerInput input;
            inputBuffer.GetDataAtTick(inputTick, out input);
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

[UpdateInGroup(typeof(ClientSimulationSystemGroup))]
public class ClientArrowSystem : ComponentSystem
{
    protected override void OnCreate()
    {
        // Don't start processing+sending inputs until a connection is ready
        RequireSingletonForUpdate<NetworkIdComponent>();
    }

    protected override void OnUpdate()
    {
        var board = GetSingleton<BoardDataComponent>();

        float2 cellCoord = float2.zero;
        Direction cellDirection = Direction.North;
        bool cellClicked = false;
        float3 screenPos = float3.zero;
        if (HasSingleton<ThinClientComponent>() && HasSingleton<LocalPlayerComponent>())
        {
            var random = new Random();
            var playerId = GetSingleton<LocalPlayerComponent>().PlayerId + 1;
            random.InitState((uint)(Time.time*10000 * playerId));

            var shouldClick = random.NextFloat(0, 1);
            var playerEntity = GetSingletonEntity<LocalPlayerComponent>();
            var arrowData = GetSingleton<AiPlayerComponent>();
            var currentPosition = arrowData.CurrentPosition;
            if (arrowData.StartTime > 0f && Math.Abs(currentPosition.x - arrowData.TargetPosition.x) < 0.1f &&
                Math.Abs(currentPosition.y - arrowData.TargetPosition.y) < 0.1f)
            {
                cellClicked = true;
                cellDirection = arrowData.Direction;
                cellCoord = arrowData.CellCoordinate;
                arrowData.StartTime = 0f;
            }
            else if (arrowData.StartTime + 2f < Time.time && shouldClick > 0.95f)
            {
                Helpers.GetRandomArrowPlacement(out var nextPosition, out var nextDirection, out cellCoord, board, playerId, random);
                arrowData.TargetPosition = Camera.main.WorldToScreenPoint(nextPosition);
                arrowData.Direction = nextDirection;
                arrowData.CellCoordinate = cellCoord;
                arrowData.StartTime = Time.time;
                EntityManager.SetComponentData(playerEntity, arrowData);
            }

            if (arrowData.TargetPosition.x == 0 && arrowData.TargetPosition.z == 0)
                return;

            // Gradually move towards the position of the next arrow placement
            Vector2 currentVelocity = Vector2.zero;
            var currentScreenPos = Vector2.SmoothDamp(
                new Vector2(currentPosition.x, currentPosition.y), new Vector2(arrowData.TargetPosition.x, arrowData.TargetPosition.y), ref currentVelocity,
                0.01f, 400f, Time.deltaTime);
            arrowData.CurrentPosition = new float3(currentScreenPos.x, currentScreenPos.y, 0f);
            screenPos = arrowData.CurrentPosition;
            EntityManager.SetComponentData(playerEntity, arrowData);
        }
        else
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
            var cellEntities = EntityManager.CreateEntityQuery(typeof(CellRenderingComponentTag)).ToEntityArray(Allocator.TempJob);
            for (int i = 0; i < cellEntities.Length; ++i)
            {
                var position = EntityManager.GetComponentData<Translation>(cellEntities[i]);
                var localPt = new float2(position.Value.x, position.Value.z) + board.cellSize * 0.5f;
                var rendCellCoord = new float2(Mathf.FloorToInt(localPt.x / board.cellSize.x), Mathf.FloorToInt(localPt.y / board.cellSize.y));
                var rendCellIndex = (int)(rendCellCoord.y * board.size.x + rendCellCoord.x);
                if (rendCellIndex == cellIndex)
                    EntityManager.AddComponentData(cellEntities[i], new ArrowComponent
                    {
                        Coordinates = cellCoord,
                        Direction = cellDirection,
                        PlacementTick = Time.time
                    });
            }
            cellEntities.Dispose();
        }

        // Set up the command target if it's not been set up yet (player was recently spawned)
        var localInput = GetSingleton<CommandTargetComponent>().targetEntity;
        if (localInput == Entity.Null)
        {
            var localPlayerId = GetSingleton<NetworkIdComponent>().Value;
            Entities.WithNone<PlayerInput>().ForEach((Entity ent, ref PlayerComponent player) =>
            {
                if (player.PlayerId == localPlayerId)
                {
                    PostUpdateCommands.AddBuffer<PlayerInput>(ent);
                    PostUpdateCommands.SetComponent(GetSingletonEntity<CommandTargetComponent>(), new CommandTargetComponent {targetEntity = ent});
                    // Atm this is the same as the PlayerComponent on the entity with the PlayerInput
                    PostUpdateCommands.AddComponent(ent, new LocalPlayerComponent{PlayerId = localPlayerId});
                }
            });
            return;
        }

        // Send input to server
        var input = default(PlayerInput);
        input.ScreenPosition = screenPos;
        input.Direction = cellDirection;
        input.CellCoordinates = (int2)cellCoord;
        input.Clicked = cellClicked;
        input.tick = World.GetExistingSystem<ClientSimulationSystemGroup>().ServerTick;
        var inputBuffer = EntityManager.GetBuffer<PlayerInput>(localInput);
        inputBuffer.AddCommandData(input);
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

public class LabRatSendCommandSystem : CommandSendSystem<PlayerInput>
{
}
public class LabRatReceiveCommandSystem : CommandReceiveSystem<PlayerInput>
{
}