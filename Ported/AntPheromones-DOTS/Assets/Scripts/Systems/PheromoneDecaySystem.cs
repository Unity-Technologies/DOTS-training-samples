using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class PheromoneDecaySystem : SystemBase
{
    public struct BufferJobExample: IJob
    {
        public BufferTypeHandle<Pheromones> pheromonesType;

        [NativeDisableContainerSafetyRestriction]
        public DynamicBuffer<Pheromones> pheromoneGrid;
        
        public void Execute()
        {
            for (int i = 0; i < pheromoneGrid.Length; i++)
            {
                float currentStrength = pheromoneGrid[i].pheromoneStrength;
                
                pheromoneGrid[i] = new Pheromones{pheromoneStrength = currentStrength - 100f};
            }
        }
    }

    protected override void OnUpdate()
    {
        Entity pheromoneEntity = GetSingletonEntity<Pheromones>();
        DynamicBuffer<Pheromones> pheromoneGrid = EntityManager.GetBuffer<Pheromones>(pheromoneEntity);
        
        BufferTypeHandle<Pheromones> pheromoneBufferType = GetBufferTypeHandle<Pheromones>();

        BufferJobExample job = new BufferJobExample
        {
            pheromonesType = pheromoneBufferType,
            pheromoneGrid = pheromoneGrid,

        };

        Dependency = job.Schedule(Dependency);
    }
}
