using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Jobs;

public struct ClosestFireRequest : IComponentData
{
    public float2 requestPosition;
    public float2 closestFirePosition;
    public int requestResultIndex;

    public ClosestFireRequest(float2 position)
    {
        requestPosition = position;
        closestFirePosition = float2.zero;
        requestResultIndex = 0;
    }
}

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(FireSimulationPropagationSystem))]
public class ClosestFireSystem : SystemBase
{
    EntityQuery m_ClosestFireRequestsQuery;

    struct ParallelReductionJob : IJobParallelFor
    {
        [Unity.Collections.LowLevel.Unsafe.NativeDisableContainerSafetyRestriction]
        public NativeArray<ReductionStructure> distances;
        public int cellPerJobCount;
        public int totalCellCount;
        public int stride;

        public void Execute(int index)
        {
            int minIndex = index * stride;
            ReductionStructure result = new ReductionStructure();
            result.distance = UnityEngine.Mathf.Infinity;
            result.index = -1;

            int innerStride = stride / cellPerJobCount;
            for (int i = 0; i < cellPerJobCount; ++i)
            {
                // TODO: Better manage partial job to avoid branch each iteration.
                int finalIndex = minIndex + (innerStride * i);
                if (finalIndex < totalCellCount)
                {
                    var current = distances[minIndex + (innerStride * i)];
                    if (current.distance <= result.distance)
                    {
                        result = current;
                    }
                }
            }

            // Store result in first index
            distances[minIndex] = result;
        }
    }

    struct WriteFinalResultJob : IJob
    {
        public int resultIndex;
        [ReadOnly]
        [DeallocateOnJobCompletion]
        public NativeArray<ReductionStructure> distances;
        public NativeArray<ReductionStructure> results;

        public void Execute()
        {
            results[resultIndex] = distances[0];
        }
    }

    struct ReductionStructure
    {
        public float distance;
        public int index;
    }

    protected override void OnCreate()
    {
        base.OnCreate();

        m_ClosestFireRequestsQuery = GetEntityQuery(typeof(ClosestFireRequest));
        RequireForUpdate(m_ClosestFireRequestsQuery);
    }

    protected override void OnUpdate()
    {
        // Prepare the result array and update requests with the result index.
        // Array should never be empty given the Required Query for update in OnCreate
        var requests = m_ClosestFireRequestsQuery.ToEntityArray(Allocator.TempJob);
        NativeArray<ReductionStructure> finalResults = new NativeArray<ReductionStructure>(requests.Length, Allocator.TempJob);
        for (int i = 0; i < requests.Length; ++i)
        {
            var requestEntity = requests[i];
            var request = GetComponent<ClosestFireRequest>(requestEntity);
            EntityManager.SetComponentData(requestEntity, new ClosestFireRequest { requestPosition = request.requestPosition, closestFirePosition = float2.zero, requestResultIndex = i });
        }

        var simulationEntity = GetSingletonEntity<FireSimulation>();
        var simulation = GetComponent<FireSimulation>(simulationEntity);
        float cellSize = simulation.cellSize;
        var simulationTemperatures = GetBufferFromEntity<SimulationTemperature>(true)[simulationEntity];
        var simMatrix = GetComponent<Unity.Transforms.LocalToWorld>(simulationEntity);
        int cellCount = simulation.rows * simulation.columns;
        NativeArray<ReductionStructure> distances = new NativeArray<ReductionStructure>(cellCount, Allocator.TempJob);

        for (int i = 0; i < requests.Length; ++i)
        {
            var request = GetComponent<ClosestFireRequest>(requests[i]);

            // TODO: See why this does not work in world space. As it is now it will only work if the simulation is at (0,0)
            //float4 requestPositionSimSpace4 = math.mul(math.inverse(simMatrix.Value), new float4(request.requestPosition.x, 0.0f, request.requestPosition.y, 1.0f));
            //float2 requestPositionSimSpace = new float2(requestPositionSimSpace4.x, requestPositionSimSpace4.y);

            float2 requestPositionSimSpace = new float2(request.requestPosition.x, request.requestPosition.y);

            // First compute all distances to the request point.
            Dependency = Entities.
                WithReadOnly(simulationTemperatures).
                ForEach((in CellIndex cellIndex) =>
                {
                    FireUtils.ArrayToGridCoord(cellIndex.Value, simulation.rows, out int row, out int column);
                    float distance = UnityEngine.Mathf.Infinity;
                    if (simulationTemperatures[cellIndex.Value] >= simulation.flashpoint) // OnFire
                {
                        float2 position = new float2(row * cellSize, column * cellSize);
                        distance = math.distancesq(position, requestPositionSimSpace);
                    }

                    ReductionStructure result = new ReductionStructure();
                    result.distance = distance;
                    result.index = cellIndex.Value;
                    distances[cellIndex.Value] = result;

                }).ScheduleParallel(Dependency);

            // Do parallel reduction to get the closest.
            int cellPerJobCount = 16;
            int jobCount = (cellCount + cellPerJobCount - 1) / cellPerJobCount;

            int currentStride = cellPerJobCount;
            var reducJob = new ParallelReductionJob()
            {
                stride = currentStride,
                cellPerJobCount = cellPerJobCount,
                totalCellCount = cellCount,
                distances = distances
            };

            while (jobCount > 0)
            {
                Dependency = reducJob.Schedule(jobCount, cellPerJobCount, Dependency);

                if (jobCount == 1)
                    break;

                currentStride *= cellPerJobCount;
                reducJob.stride = currentStride;
                jobCount = (jobCount + cellPerJobCount - 1) / cellPerJobCount;
            }

            // Write in finalResults array.
            var writeFinalJob = new WriteFinalResultJob()
            {
                resultIndex = request.requestResultIndex,
                distances = distances,
                results = finalResults
            };

            Dependency = writeFinalJob.Schedule(Dependency);
        }

        Dependency = Entities
            .WithDisposeOnCompletion(finalResults)
            .ForEach((ref ClosestFireRequest request) =>
        {
            var resultCellIndex = finalResults[request.requestResultIndex].index;
            FireUtils.ArrayToGridCoord(resultCellIndex, simulation.rows, out int row, out int column);
            float2 resultSimSpace = new float2(row * cellSize, column * cellSize);
            var result = new float4(resultSimSpace.x, 0.0f, resultSimSpace.y, 1.0f);
            //result = math.mul(simMatrix.Value, result);
            request.closestFirePosition = result.xz;
        }).Schedule(Dependency);

        requests.Dispose();

        //Dependency.Complete();
        //var requestResult = GetSingleton<ClosestFireRequest>();
        //UnityEngine.Debug.Log(requestResult.closestFirePosition);
    }
}
