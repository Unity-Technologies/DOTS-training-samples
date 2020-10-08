using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;

public struct CloestFireRequest
{
    public float2 requestPosition;
    public float2 closestFirePosition;
}

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(FireSimulationPropagationSystem))]
public class ClosestFireSystem : SystemBase
{
    struct ParallelReductionJob
    {

    }

    protected override void OnUpdate()
    {
        //float2 requestPosition = new float2(5.4f, 24.2f);

        //var simulationEntity = GetSingletonEntity<FireSimulation>();
        //var simulation = GetComponent<FireSimulation>(simulationEntity);
        //float cellSize = simulation.cellSize;
        //var simulationTemperatures = GetBufferFromEntity<SimulationTemperature>(true)[simulationEntity];
        //var simMatrix = GetComponent<Unity.Transforms.LocalToWorld>(simulationEntity);
        //float4 requestPositionSimSpace4 = math.mul(math.inverse(simMatrix.Value), new float4(requestPosition.x, requestPosition.y, 0.0f, 1.0f));
        //float2 requestPositionSimSpace = new float2(requestPositionSimSpace4.x, requestPositionSimSpace4.y);

        //int cellCount = simulation.rows * simulation.columns;
        //NativeArray<float> distances = new NativeArray<float>(cellCount, Allocator.TempJob);

        //// First compute all distances to the request point.
        //Dependency = Entities.
        //    WithReadOnly(simulationTemperatures).
        //    ForEach((in CellIndex cellIndex) =>
        //    {
        //        FireUtils.ArrayToGridCoord(cellIndex.Value, simulation.rows, out int row, out int column);
        //        float result = UnityEngine.Mathf.Infinity;
        //        if (simulationTemperatures[cellIndex.Value] >=  simulation.flashpoint) // OnFire
        //        {
        //            float2 position = new float2(row * cellSize, column * cellSize);
        //            result = math.distancesq(position, requestPositionSimSpace);
        //        }

        //        distances[cellIndex.Value] = result;

        //    }).ScheduleParallel(Dependency);

        //// Do parallel reduction to get the closest.


        ////Dependency.Complete();
        //distances.Dispose();
    }
}
