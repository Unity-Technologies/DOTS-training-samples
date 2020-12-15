using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class PheromoneTrackingSystem : SystemBase
{
    public struct BufferJobExample: IJobEntityBatch
    {
        public BufferTypeHandle<Pheromones> pheromonesType;
        [ReadOnly]
        public ComponentTypeHandle<Translation> translationType;
        [NativeDisableContainerSafetyRestriction]
        public DynamicBuffer<Pheromones> pheromoneGrid;
        
        public void Execute(ArchetypeChunk batchInChunk, int batchIndex)
        {
            var translations = batchInChunk.GetNativeArray(translationType);
            
            /*for (int i = 0; i < batchInChunk.Count; i++)
            {
                var translation = translations[i];
                float currentStrength = pheromoneGrid[i].pheromoneStrength;
                
                pheromoneGrid[0] = new Pheromones{pheromoneStrength = currentStrength + 10f};
            }*/
            
            float currentStrength = pheromoneGrid[0].pheromoneStrength;
                
            pheromoneGrid[0] = new Pheromones{pheromoneStrength = currentStrength + 10f};
        }
    }

    protected override void OnUpdate()
    {
        Entity pheromoneEntity = GetSingletonEntity<Pheromones>();
        DynamicBuffer<Pheromones> pheromoneGrid = EntityManager.GetBuffer<Pheromones>(pheromoneEntity);

        EntityQuery query = GetEntityQuery(typeof(Translation), typeof(Ant));
        BufferTypeHandle<Pheromones> pheromoneBufferType = GetBufferTypeHandle<Pheromones>();
        ComponentTypeHandle<Translation> translationType = GetComponentTypeHandle<Translation>(true);

        BufferJobExample job = new BufferJobExample
        {
            pheromonesType = pheromoneBufferType,
            pheromoneGrid = pheromoneGrid,
            translationType = translationType
        };

        Dependency = job.Schedule(query, Dependency);
    }
}
