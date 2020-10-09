using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(InitializationSystemGroup))]
[UpdateAfter(typeof(Unity.Scenes.SceneSystemGroup))]
public class FireSimulationInitSystem : SystemBase
{
    Random m_Random;

    protected override void OnCreate()
    {
        // We only want all this to be called once for the singleton simulation.
        // (Overrides second query in the update that will be run every frame).
        RequireForUpdate(GetEntityQuery(new EntityQueryDesc
        {
            None = new ComponentType[] { typeof(FireSimulationStarted) },
            All = new ComponentType[] { typeof(FireSimulation) }
        }));

        m_Random = new Random((uint)System.DateTime.UtcNow.Millisecond);
    }

    protected override void OnUpdate()
    {
        // Initialize fire cells
        Entities.
            WithStructuralChanges().
            WithNone<FireSimulationStarted>().
            ForEach((Entity fireSimulationEntity, in FireSimulation simulation, in Translation simulationTranslation, in Rotation simulationRotation) =>
        {
            int cellCount = simulation.rows * simulation.columns;
            float cellSize = simulation.cellSize;

            var entities = EntityManager.Instantiate(simulation.fireCellPrefab, cellCount, Unity.Collections.Allocator.TempJob);

            var neutralColor = new float4(simulation.fireCellColorNeutral.r, simulation.fireCellColorNeutral.g, simulation.fireCellColorNeutral.b, simulation.fireCellColorNeutral.a);

            for (int y = 0; y < simulation.columns; ++y)
            {
                for (int x = 0; x < simulation.rows; ++x)
                {
                    int cellIndex = FireUtils.GridToArrayCoord(x, y, simulation.rows);
                    var entity = entities[cellIndex];
                    float posX = x * cellSize;
                    float posY = -(simulation.maxFlameHeight * 0.5f) + m_Random.NextFloat(0.01f, 0.02f);
                    float posZ = y * cellSize;
                    var translation = simulationTranslation.Value + new float3(posX, posY, posZ);
                    EntityManager.SetComponentData(entity, new Translation { Value = translation });
                    EntityManager.SetComponentData(entity, simulationRotation);
                    EntityManager.AddComponent<NonUniformScale>(entity);
                    EntityManager.SetComponentData(entity, new NonUniformScale { Value = new float3(cellSize, simulation.maxFlameHeight, cellSize) });
                    EntityManager.SetComponentData(entity, new BaseColor { Value = neutralColor });

                    EntityManager.AddComponent<Temperature>(entity);
                    EntityManager.AddComponent<CellIndex>(entity);
                    EntityManager.SetComponentData(entity, new CellIndex { Value = cellIndex });
                }
            }

            // Set on fire a few cells.
            for (int i = 0; i < simulation.startingFireCount; i++)
            {
                int index = m_Random.NextInt(0, cellCount);
                var entity = entities[index];
                float newTemperature = m_Random.NextFloat(simulation.flashpoint, 1.0f);
                EntityManager.SetComponentData(entity, new Temperature { Value = newTemperature });
            }

            // Add simulation timer.
            EntityManager.AddComponent<Timer>(fireSimulationEntity);
            EntityManager.SetComponentData(fireSimulationEntity, new Timer { elapsedTime = 0.0f, timerValue = simulation.fireSimUpdateRate });

            EntityManager.AddComponent<FireSimulationStarted>(fireSimulationEntity);

            // TEST TEMP
            //var request = EntityManager.CreateEntity();
            //EntityManager.AddComponentData(request, new ClosestFireRequest(new float2(5.5f, 9.4f)));

            entities.Dispose();
        }).Run();

        // Fill initial simulation temperatures.
        var simulationEntity = GetSingletonEntity<FireSimulation>();
        var fireSimulation = GetComponent<FireSimulation>(simulationEntity);
        var temperatureBuffer = GetBuffer<SimulationTemperature>(simulationEntity);
        temperatureBuffer.ResizeUninitialized(fireSimulation.columns * fireSimulation.rows);
        var temperatureBufferNative = temperatureBuffer.AsNativeArray();

        Entities.
            ForEach((in Temperature temperature, in CellIndex cellIndex) =>
            {
                temperatureBufferNative[cellIndex.Value] = temperature.Value;
            }).ScheduleParallel();
    }
}
