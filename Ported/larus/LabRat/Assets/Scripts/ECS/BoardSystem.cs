using ECSExamples;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;

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

        // TODO: Use better place to define player to color mappings
        var color = PlayerCursor.PlayerColors[playerId-1];

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
}
