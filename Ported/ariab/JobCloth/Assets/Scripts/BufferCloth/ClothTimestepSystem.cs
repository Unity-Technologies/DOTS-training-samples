using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

struct ClothTimestepSingleton : IComponentData
{
    public int IntegrationCount;
}

// See ClothSolverSystemGroup
[DisableAutoCreation]
[AlwaysUpdateSystem]
public class ClothTimestepSystem : ComponentSystem
{
    private EntityQuery ClothTimeSingletonQuery;
    
    protected override void OnCreate()
    {
        ClothTimeSingletonQuery = GetEntityQuery(typeof(ClothTimestepSingleton));
    }

    protected override void OnUpdate()
    {
        if (ClothTimeSingletonQuery.CalculateEntityCount() == 0)
            EntityManager.CreateEntity(typeof(ClothTimestepSingleton));
        
        // hack: use the highest number of iterations for all cloth for simple scheduling. DOTS needs scheduling jobs from jobs :(
        var mostIterations = 0;
        
        Entities.ForEach((ref ClothTotalTime timeData, ref ClothTimestepData fixedTimeStep) =>
        {
            var totalTime = timeData.TotalTime;
            var newTotalTime = totalTime + Time.DeltaTime;

            var integrationCount = 0;
            while (newTotalTime > fixedTimeStep.FixedTimestep)
            {
                newTotalTime -= fixedTimeStep.FixedTimestep;
                integrationCount++;
            }
            integrationCount = math.min(integrationCount, 4);
            mostIterations = math.max(mostIterations, integrationCount);

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
        
        SetSingleton(new ClothTimestepSingleton{IntegrationCount = mostIterations});
    }
}