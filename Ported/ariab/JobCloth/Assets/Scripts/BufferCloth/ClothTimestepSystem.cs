using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public class ClothTimestepSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((ref ClothTotalTime timeData, ref ClothTimestepData fixedTimeStep) =>
        {
            var totalTime = timeData.TotalTime;
            var newTotalTime = totalTime + Time.deltaTime;

            var integrationCount = 0;
            while (newTotalTime > fixedTimeStep.FixedTimestep)
            {
                newTotalTime -= fixedTimeStep.FixedTimestep;
                integrationCount++;
            }
            integrationCount = math.min(integrationCount, 4);

            // Write back time data
            timeData = new ClothTotalTime
            {
                TotalTime = newTotalTime,
            };

            fixedTimeStep = new ClothTimestepData
            {
                FixedTimestep = fixedTimeStep.FixedTimestep,
                IterationCount = integrationCount
            };
        });
    }
}