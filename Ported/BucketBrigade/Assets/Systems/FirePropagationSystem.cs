using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class FirePropagationSystem : SystemBase
{
    public NativeArray<float> cells;
    NativeArray<float> tempCells;

    protected override void OnStartRunning()
    {
        base.OnStartRunning();
        FireSim fireSim = GetSingleton<FireSim>();
        int numCells = fireSim.FireGridDimension * fireSim.FireGridDimension;

        cells = new NativeArray<float>(numCells, Allocator.Persistent);
        tempCells = new NativeArray<float>(numCells, Allocator.Persistent);

        var fireCells = cells;
        var random = new Unity.Mathematics.Random(42);

        Entities
            .ForEach((ref FireCell fireCell, in Translation translation) =>
            {
                int2 index = new int2((int)translation.Value.x, (int)translation.Value.z);
                if (random.NextFloat() < fireSim.IgnitionRate)
                {
                    float randomTemperature = random.NextFloat(fireSim.FlashPoint, 1.0f);
                    fireCells[index.x * fireSim.FireGridDimension + index.y] = randomTemperature;
                    fireCell.Temperature = randomTemperature;
                }
                else
                {
                    fireCells[index.x * fireSim.FireGridDimension + index.y] = 0;
                }

            }).ScheduleParallel();
    }

    protected override void OnUpdate()
    {
        FireSim fireSim = GetSingleton<FireSim>();
        var fireCells = cells;

        Entities
            .WithReadOnly(fireCells)
            .ForEach((ref FireCell fireCell, in Translation translation) =>
        {
            int2 index = new int2((int)translation.Value.x, (int)translation.Value.z);
            fireCell.Temperature = fireCells[index.x * fireSim.FireGridDimension + index.y];
        }).ScheduleParallel();

        // Set up the job data
        int numCells = fireSim.FireGridDimension * fireSim.FireGridDimension;

        var propagationJob = new PropagationJob();
        propagationJob.InputCells = fireCells;
        propagationJob.OutputCells = tempCells;
        propagationJob.PropagationRadius = fireSim.PropagationRadius;
        propagationJob.FireGridDimension = fireSim.FireGridDimension;
        propagationJob.HeatTransfer = fireSim.HeatTransfer;

        // Schedule the job
        Dependency = propagationJob.Schedule(numCells, fireSim.FireGridDimension, Dependency);

        // Swap the two buffers
        cells = tempCells;
        tempCells = fireCells;
    }

    protected override void OnStopRunning()
    {
        cells.Dispose();
        tempCells.Dispose();
        base.OnDestroy();
    }
}
