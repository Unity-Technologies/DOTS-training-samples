using Unity.Entities;

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(FireSimulationUpdateSystem))]
public class FireSimulationPropagationSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var simulationEntity = GetSingletonEntity<FireSimulation>();
        Timer timer = GetComponent<Timer>(simulationEntity);

        if (timer.TimerIsUp())
        {
            //FireSimulation fireSimulation = GetComponent<FireSimulation>(simulationEntity);
            //var simulationTemperatures = GetBuffer<SimulationTemperature>(simulationEntity).AsNativeArray();

            //Entities.
            //        ForEach((Entity fireCellEntity, ref Temperature temperature, in CellIndex cellIndex) =>
            //        {
            //            simulationTemperatures[cellIndex.Value] += 0.1f;
            //        }).ScheduleParallel();
        }
    }
}
