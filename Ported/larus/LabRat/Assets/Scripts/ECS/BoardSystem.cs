using Unity.Collections;
using Unity.Entities;

//[UpdateBefore(typeof(GameObjectBeforeConversionGroup))]
public class BoardSystem : ComponentSystem
{
    public NativeHashMap<int, CellComponent> CellMap => m_CellMap;
    public NativeHashMap<int, int> HomeBase => m_HomebaseMap;

    private NativeHashMap<int, CellComponent> m_CellMap;
    private NativeHashMap<int, int> m_HomebaseMap;

    protected override void OnCreate()
    {
        m_CellMap = new NativeHashMap<int, CellComponent>(2048, Allocator.Persistent);
        m_HomebaseMap = new NativeHashMap<int, int>(4, Allocator.Persistent);
    }

    protected override void OnDestroy()
    {
        m_CellMap.Dispose();
        m_HomebaseMap.Dispose();
    }

    protected override void OnUpdate()
    {

    }
}
