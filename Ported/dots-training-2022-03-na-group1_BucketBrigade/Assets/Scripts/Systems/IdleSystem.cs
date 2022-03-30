using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

public partial class IdleSystem : SystemBase
{
    private static Random _random; // causes issues but just used for debugging RN.
    
    protected override void OnCreate()
    {
        base.OnCreate();

        _random = new Random(1234); // TODO seed properly.
    }

    protected override void OnUpdate()
    {
        // The PRNG (pseudorandom number generator) from Unity.Mathematics is a struct
        // and can be used in jobs. For simplicity and debuggability in development,
        // we'll initialize it with a constant. (In release, we'd want a seed that
        // randomly varies, such as the time from the user's system clock.)

        var heatMapDataEntity = GetSingletonEntity<HeatMapData>();
        var heatMapData = GetComponent<HeatMapData>(heatMapDataEntity);
        
        var radius = (heatMapData.mapSideLength - 1) / 2f;

        Entities
            .ForEach((ref MyWorkerState state, ref RelocatePosition destination) =>
            {
                if (state.Value == WorkerState.Idle)
                {
                    if (_random.NextFloat() < 0.01f)
                    {
                        destination.Value = new float2(_random.NextFloat(-radius, radius),
                            _random.NextFloat(-radius, radius));
                    }
                }
            }).ScheduleParallel();
    }
}
