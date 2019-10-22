using Unity.Collections;
using Unity.Entities;

//[UpdateBefore(typeof(GameObjectBeforeConversionGroup))]
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
}
