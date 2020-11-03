using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

public class FirePropagationSystem : SystemBase
{
    public NativeArray<byte> cells;
    protected override void OnCreate()
    {
        FireSim fireSim = GetSingleton<FireSim>();
        cells = new NativeArray<byte>(fireSim.FireGridDimension*fireSim.FireGridDimension, Allocator.Persistent);
    }

    protected override void OnUpdate()
    {
        var cellcopy = cells;
        FireSim fireSim = GetSingleton<FireSim>();
        Entities.ForEach((ref FireCell fireCell, in Translation translation) =>
        {
            fireCell.Temperature = cellcopy[(int)(translation.Value.x * fireSim.FireGridDimension + translation.Value.z)];
        }).ScheduleParallel();
    }

    protected override void OnDestroy()
    {
        cells.Dispose();
        base.OnDestroy();
    }
}
