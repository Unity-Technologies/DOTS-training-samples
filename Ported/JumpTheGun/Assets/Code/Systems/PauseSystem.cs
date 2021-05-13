
using Unity.Core;
using Unity.Entities;

[UpdateInGroup(typeof(Unity.Entities.SimulationSystemGroup))]
public class PauseSystem : SystemBase
{
    protected override void OnCreate()
    {
        var query = GetEntityQuery(typeof(IsPaused));
        RequireForUpdate(query);
    }

    protected override void OnUpdate()
    {
        if (!TryGetSingleton<IsPaused>(out IsPaused pause))
            return;

        World.DefaultGameObjectInjectionWorld.SetTime(new TimeData(pause.Time, 0));
    }
}
