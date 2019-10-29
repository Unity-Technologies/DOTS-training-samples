using ECSExamples;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Networking.Transport;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

public struct PlayerInput : ICommandData<PlayerInput>
{
    public int2 CellCoordinates;
    public Direction Direction;
    public bool Clicked;

    public uint Tick => tick;
    public uint tick;

    public void Serialize(DataStreamWriter writer)
    {
        writer.Write(CellCoordinates.x);
        writer.Write(CellCoordinates.y);
        writer.Write((byte)Direction);
        writer.Write((byte)(Clicked ? 1 : 0));
    }

    public void Deserialize(uint tick, DataStreamReader reader, ref DataStreamReader.Context ctx)
    {
        this.tick = tick;
        var x = reader.ReadInt(ref ctx);
        var y = reader.ReadInt(ref ctx);
        CellCoordinates = new int2(x, y);
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

        // Human always player 0
        //const int humanPlayerIndex = 0;
        //var canChangeArrow = cell.CanChangeArrow(humanPlayerIndex);

        var overlayEntities = GetEntityQuery(typeof(OverlayComponentTag), typeof(Translation)).ToEntityArray(Allocator.Persistent);
        var overlayPositions = GetEntityQuery(typeof(OverlayComponentTag), typeof(Translation)).ToComponentDataArray<Translation>(Allocator.Persistent);
        var overlayRotations = GetEntityQuery(typeof(OverlayComponentTag), typeof(Rotation)).ToComponentDataArray<Rotation>(Allocator.Persistent);
        var overlayPlacementTick = GetEntityQuery(typeof(OverlayComponentTag), typeof(OverlayPlacementTickComponent))
            .ToComponentDataArray<OverlayPlacementTickComponent>(Allocator.Persistent);

        var overlayColorEntities = GetEntityQuery(typeof(OverlayColorComponent), typeof(Translation)).ToEntityArray(Allocator.Persistent);
        var keys = arrowMap.GetKeyArray(Allocator.TempJob);
        //var overlayColorPositions = GetEntityQuery(typeof(OverlayColorComponentTag), typeof(Translation)).ToComponentDataArray<Translation>(Allocator.TempJob);

        // SAMPLE INPUT BUFFER
        Entities.ForEach((DynamicBuffer<PlayerInput> inputBuffer, ref PlayerComponent player) =>
        {
            var simulationSystemGroup = World.GetExistingSystem<ServerSimulationSystemGroup>();
            var inputTick = simulationSystemGroup.ServerTick;
            PlayerInput input;
            inputBuffer.GetDataAtTick(inputTick, out input);
            if (input.Clicked)
            {
                var cellIndex = input.CellCoordinates.y * board.size.y + input.CellCoordinates.x;

                // Update the cell data and arrow map with this arrow placement, used by game logic
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
                    Coordinates = input.CellCoordinates,
                    Direction = input.Direction,
                    PlayerId = player.PlayerId,
                    PlacementTick = Time.time
                };

                int arrowCount = 0;
                int oldestIndex = -1;
                float oldestTick = float.MaxValue;
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
                Debug.Log("Placed arrow on cell="+input.CellCoordinates + " with dir=" + input.Direction);

                // Update the overlays (arrow+color) which shows the player visually where the arrow is placed
                var startIndex = player.PlayerId * PlayerConstants.MaxArrows;
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
        });
        keys.Dispose();
        overlayPlacementTick.Dispose();
        overlayColorEntities.Dispose();
        //overlayColorPositions.Dispose();
        overlayRotations.Dispose();
        overlayPositions.Dispose();
        overlayEntities.Dispose();

        // Wall building stuff
        //if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            //cell.ToggleWall(dir);

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


        // TODO: If this exists, generate dummy input
        //HasSingleton<ThinClientComponent>()

        // Input gathering
        var screenPos = Input.mousePosition;
        float2 cellCoord;
        int cellIndex;
        float3 worldPos;
        Helpers.ScreenPositionToCell(screenPos, board.cellSize, board.size, out worldPos, out cellCoord, out cellIndex);

        Direction cellDirection;
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
                }
            });
            return;
        }

        // TODO: Also do dummy input generation for non-presenting client
        // Send input to server
        var input = default(PlayerInput);
        input.Direction = cellDirection;
        input.CellCoordinates = (int2)cellCoord;
        input.Clicked = Input.GetMouseButtonDown(0);
        input.tick = World.GetExistingSystem<ClientSimulationSystemGroup>().ServerTick;
        var inputBuffer = EntityManager.GetBuffer<PlayerInput>(localInput);
        inputBuffer.AddCommandData(input);

        // Input data:
        //    ArrowDirection
        //    CellIndex
        //    Clicked (if mouse should be stamped)
        //    (Player ID) - server could figure that out from input sender
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
}

public class LabRatSendCommandSystem : CommandSendSystem<PlayerInput>
{
}
public class LabRatReceiveCommandSystem : CommandReceiveSystem<PlayerInput>
{
}