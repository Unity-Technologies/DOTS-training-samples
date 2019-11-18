using ECSExamples;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

[UpdateInGroup(typeof(ServerSimulationSystemGroup))]
public class BoardSystem : ComponentSystem
{
    public NativeHashMap<int, CellComponent> CellMap => m_CellMap;
    public NativeHashMap<int, int> HomeBaseMap => m_HomebaseMap;
    public NativeHashMap<int, int> CatMap => m_CatMap;
    public NativeHashMap<int, ArrowComponent> ArrowMap => m_ArrowMap;

    private NativeHashMap<int, CellComponent> m_CellMap;
    private NativeHashMap<int, int> m_HomebaseMap;
    private NativeHashMap<int, int> m_CatMap;
    private NativeHashMap<int, ArrowComponent> m_ArrowMap;

    public struct InitializedBoard : IComponentData
    {}

    public struct InitializedCellData : IComponentData
    {}

    protected override void OnCreate()
    {
        m_CellMap = new NativeHashMap<int, CellComponent>(2048, Allocator.Persistent);
        m_HomebaseMap = new NativeHashMap<int, int>(4, Allocator.Persistent);
        m_CatMap = new NativeHashMap<int, int>(64, Allocator.Persistent);
        m_ArrowMap = new NativeHashMap<int, ArrowComponent>(64, Allocator.Persistent);
    }

    protected override void OnDestroy()
    {
        m_CellMap.Dispose();
        m_HomebaseMap.Dispose();
        m_CatMap.Dispose();
        m_ArrowMap.Dispose();
    }

    protected override void OnUpdate()
    {
        // Move source board data into the runtime layout (hashmap)
        Entities.WithNone<InitializedCellData>().ForEach((Entity entity, DynamicBuffer<BoardCellData> cellData) =>
        {
	        for (int i = 0; i < cellData.Length; ++i)
	        {
		        if (cellData[i].cellData != 0)
					m_CellMap.Add(i, new CellComponent{data = cellData[i].cellData});
	        }
	        PostUpdateCommands.AddComponent<InitializedCellData>(entity);
        });
        // Generate the walls and holes placed on the board
        Entities.WithNone<InitializedBoard>().ForEach((Entity entity, ref BoardDataComponent board) =>
        {
	        var random = new Random();
	        random.InitState((uint)((Time.ElapsedTime+1)*10000));
	        GenerateBoardData(entity, board, random);
	        PostUpdateCommands.AddComponent<InitializedBoard>(entity);
        });
    }

    public void SpawnHomebase(int playerId)
    {
        var board = GetSingleton<BoardDataComponent>();
        float offsetX = board.size.x * 1f / 3f;
        float offsetY = board.size.y * 1f / 3f;
        switch (playerId)
        {
            case 2:
                offsetX *= 2f;
                offsetY *= 2f;
                break;
            case 3:
                offsetY *= 2f;
                break;
            case 4:
                offsetX *= 2f;
                break;
        }
        var x = math.floor(offsetX);
        var y = math.floor(offsetY);

        var colors = World.GetOrCreateSystem<ApplyOverlayColors>();
        var color = colors.Colors[playerId-1];

        var prefabs = GetSingleton<GhostPrefabCollectionComponent>();
        var serverPrefabs = EntityManager.GetBuffer<GhostPrefabBuffer>(prefabs.serverPrefabs);
        var prefab = serverPrefabs[LabRatGhostSerializerCollection.FindGhostType<HomebaseSnapshotData>()].Value;
        var homebase = EntityManager.Instantiate(prefab);

        EntityManager.SetComponentData(homebase, new Translation{Value = new float3(x, 0, y)});
        EntityManager.AddComponentData(homebase, new HomebaseComponent
            {Color = new float4(color.a, color.b, color.g, color.r), PlayerId = playerId});

        int cellIndex = (int)(y * board.size.y + x);
        if (m_CellMap.ContainsKey(cellIndex))
        {
            var data = m_CellMap[cellIndex].data;
            data |= CellData.HomeBase;
            m_CellMap[cellIndex] = new CellComponent{data = data};
        }
        else
            m_CellMap.Add(cellIndex, new CellComponent{data = CellData.HomeBase});
        HomeBaseMap.Add(cellIndex, playerId);
    }

    CellData SetWallData(Entity cell, CellData cellData)
    {
        if (EntityManager.HasComponent<Child>(cell))
        {
            DynamicBuffer<Child> children = EntityManager.GetBuffer<Child>(cell);
            for (int i = 0; i < children.Length - 1; ++i)
            {
                var position = EntityManager.GetComponentData<Translation>(children[i].Value);
                if (position.Value.x > 0)
                    cellData = cellData | CellData.WallEast;
                if (position.Value.x < 0)
                    cellData = cellData | CellData.WallWest;
                if (position.Value.z > 0)
                    cellData = cellData | CellData.WallSouth;
                if (position.Value.z < 0)
                    cellData = cellData | CellData.WallNorth;
            }
        }
        return cellData;
    }

    void GenerateBoardData(Entity boardEntity, BoardDataComponent board, Random random)
    {
        var boardSize = board.size;

        //var oldState = Random.state;
        if (board.randomSeed != -1)
            random.InitState((uint)board.randomSeed);

        int numWalls = (int)(boardSize.x * boardSize.y * 0.2f);
        var cells = EntityManager.CreateEntityQuery(typeof(CellRenderingComponentTag)).ToEntityArray(Allocator.TempJob);
        var cellPositions = EntityManager.CreateEntityQuery(typeof(CellRenderingComponentTag), typeof(Translation)).ToComponentDataArray<Translation>(Allocator.TempJob);
        for (int c = 0; c < numWalls; ++c)
        {
            var cellCoord = new float2(random.NextInt(boardSize.x), random.NextInt(boardSize.y));
            var cellIndex = (int)(cellCoord.y * board.size.y + cellCoord.x);
            var cellEntity = Entity.Null;
            for (int i = 0; i < cells.Length; ++i)
            {
                if (math.abs(cellPositions[i].Value.x - cellCoord.x) < 0.01f &&
                    math.abs(cellPositions[i].Value.z - cellCoord.y) < 0.01f)
                {
                    cellEntity = cells[i];
                    break;
                }
            }
            var direction = (Direction)random.NextInt(4);
            if (!PlaceWall(cellIndex, cellCoord, direction, board, cellEntity))
                c--;
        }

        var stressTest = false;
        //var config = FindObjectOfType<GameConfig>();
        var MouseSpawner = GetSingleton<PrefabCollectionComponent>().MouseSpawner;
        var CatSpawner = GetSingleton<PrefabCollectionComponent>().CatSpawner;
        SpawnerAt(MouseSpawner, 0, 0, Quaternion.identity);
        SpawnerAt(MouseSpawner, boardSize.x - 1, boardSize.y - 1, Quaternion.Euler(0, 180, 0));
        if (!stressTest)
        {
            SpawnerAt(CatSpawner, 0, boardSize.y - 1, Quaternion.Euler(0, 0, 0));
            SpawnerAt(CatSpawner, boardSize.x - 1, 0, Quaternion.Euler(0, 0, 0));
        }

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

        if (!stressTest)
        {
            int numHoles = random.NextInt(4);
            for (int i = 0; i < numHoles; ++i)
            {
                //var coord = new Vector2Int(random.NextInt(boardSize.x), random.NextInt(boardSize.y));
                var cellIndex = random.NextInt(board.size.x * board.size.y - 1);
                if (!m_CellMap.ContainsKey(cellIndex))
                {
                    //PostUpdateCommands.DestroyEntity(cells[cellIndex]);
                }
                //if (coord.x > 0 && coord.y > 0 && coord.x < boardSize.x - 1 && coord.y < boardSize.y - 1 &&
                //    CellAtCoord(coord).IsEmpty())
                //    RemoveCell(coord);
            }
        }

        cells.Dispose();
        cellPositions.Dispose();
    }

    bool CanPlaceWallHere(int index, CellData direction)
    {
        if (m_CellMap.ContainsKey(index))
        {
            if ((m_CellMap[index].data & direction) == direction)
                return false;
            var wallCount = 0;
            wallCount += (int)(m_CellMap[index].data & CellData.WallWest);
            wallCount += (int)(m_CellMap[index].data & CellData.WallEast) >> 1;
            wallCount += (int)(m_CellMap[index].data & CellData.WallNorth) >> 2;
            wallCount += (int)(m_CellMap[index].data & CellData.WallSouth) >> 3;
            if (wallCount > 2)
                return false;
        }
        return true;
    }

    bool PlaceWall(int cellIndex, float2 cellCoord, Direction direction, BoardDataComponent board, Entity cellEntity)
    {
        var wallData = WalkSystem.DirectionToCellData(direction);
        if (!CanPlaceWallHere(cellIndex, wallData))
            return false;

        if (!UpdateNeighborWall(board, cellCoord, direction))
            return false;

        SetWall(direction, cellEntity);
        if (m_CellMap.ContainsKey(cellIndex))
        {
            var data = m_CellMap[cellIndex].data;
            data |= wallData;
            m_CellMap[cellIndex] = new CellComponent{data = data};
        }
        else
        {
            m_CellMap.Add(cellIndex, new CellComponent { data = wallData });
        }

        return true;
    }

    bool UpdateNeighborWall(BoardDataComponent board, float2 coord, Direction direction)
    {
        float2 neighborCoord = float2.zero;
        int neighborIndex = 0;
        switch (direction)
        {
            case Direction.North:
                neighborCoord = coord + new float2(0,1);
                neighborIndex = (int)(neighborCoord.y * board.size.y + neighborCoord.x);
                if (neighborCoord.y >= board.size.y)
                    return true;
                if (!CanPlaceWallHere(neighborIndex, CellData.WallSouth))
                    return false;
                UpdateNeighborAtIndex(neighborIndex, CellData.WallSouth);
                break;
            case Direction.South:
                neighborCoord = coord + new float2(0,-1);
                neighborIndex = (int)(neighborCoord.y * board.size.y + neighborCoord.x);
                if (neighborCoord.y < 0)
                    return true;
                if (!CanPlaceWallHere(neighborIndex, CellData.WallNorth))
                    return false;
                UpdateNeighborAtIndex(neighborIndex, CellData.WallNorth);
                break;
            case Direction.East:
                neighborCoord = coord + new float2(1,0);
                neighborIndex = (int)(neighborCoord.y * board.size.y + neighborCoord.x);
                if (neighborCoord.x < 0)
                    return true;
                if (!CanPlaceWallHere(neighborIndex, CellData.WallEast))
                    return false;
                UpdateNeighborAtIndex(neighborIndex, CellData.WallEast);
                break;
            case Direction.West:
                neighborCoord = coord + new float2(-1,0);
                neighborIndex = (int)(neighborCoord.y * board.size.y + neighborCoord.x);
                if (neighborCoord.x >= board.size.x)
                    return true;
                if (!CanPlaceWallHere(neighborIndex, CellData.WallWest))
                    return false;
                UpdateNeighborAtIndex(neighborIndex, CellData.WallWest);
                break;
        }
        return true;
    }

    void UpdateNeighborAtIndex(int neighborIndex, CellData wallDirection)
    {
        if (m_CellMap.ContainsKey(neighborIndex))
        {
            var neighborCell = m_CellMap[neighborIndex].data;
            neighborCell |= wallDirection;
            m_CellMap[neighborIndex] = new CellComponent{data = neighborCell};
        }
        else
            m_CellMap.Add(neighborIndex, new CellComponent{data = wallDirection});
    }

    void SpawnerAt(Entity spawner, float cellX, float cellY, Quaternion rotation)
    {
        var entity = EntityManager.Instantiate(spawner);
        EntityManager.SetComponentData(entity, new Translation{Value = new float3(cellX, 0f, cellY)});
        EntityManager.SetComponentData(entity, new Rotation{Value = rotation});

        var stressTest = GetSingleton<GameConfigComponent>().StressTest;
        if (stressTest)
        {
            var spawnerComponent = EntityManager.GetComponentData<SpawnerComponent>(spawner);
            spawnerComponent.Frequency = 0.05f;
            EntityManager.SetComponentData(spawner, spawnerComponent);
        }
    }

    public void SetWall(Direction direction, Entity parent)
    {
        var mySize = new float3(1,1,1);
        var prefabs = GetSingleton<GhostPrefabCollectionComponent>();
        var serverPrefabs = EntityManager.GetBuffer<GhostPrefabBuffer>(prefabs.serverPrefabs);
        var wallPrefab = serverPrefabs[LabRatGhostSerializerCollection.FindGhostType<SpawnedWallSnapshotData>()].Value;
        var wall = EntityManager.Instantiate(wallPrefab);
        EntityManager.AddComponentData(wall, new Parent { Value = parent });
        EntityManager.AddComponentData(wall, new LocalToParent() );

#if ENABLE_UNITY_COLLECTIONS_CHECKS
        EntityManager.SetName(wall, "Wall");
#endif

        var wallScale = new float3(0.05f, 0.45f, 1f);
        var parentPosition = EntityManager.GetComponentData<Translation>(parent);
        float z = 0;
        float x = 0;
        var rotation = quaternion.identity;
        if (direction == Direction.North || direction == Direction.South)
        {
            rotation = quaternion.RotateY(math.PI/2);
            z = (direction == Direction.North ? 1f : -1f) * (mySize.z * 0.5f - wallScale.x);
        }
        else
        {
            x = (direction == Direction.East ? 1f : -1f) * (mySize.x * 0.5f - wallScale.x);
        }
        EntityManager.SetComponentData(wall, new Rotation{Value = rotation});

        var y = wallScale.y * 0.5f + mySize.y * 0.5f;
        EntityManager.SetComponentData(wall, new Translation{Value = new float3(parentPosition.Value.x + x, parentPosition.Value.y + y, parentPosition.Value.z + z)});
    }
}
