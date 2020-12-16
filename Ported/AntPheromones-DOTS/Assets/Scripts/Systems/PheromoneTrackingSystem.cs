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
        //public BufferTypeHandle<Pheromones> pheromonesType;
        [NativeDisableContainerSafetyRestriction]
        public DynamicBuffer<Pheromones> pheromoneGrid;
        
        [ReadOnly]
        public ComponentTypeHandle<Translation> translationType;
        
        public float pheromoneApplicationRate;
        public float deltaTime;
        
        public void Execute(ArchetypeChunk batchInChunk, int batchIndex)
        {
            var translations = batchInChunk.GetNativeArray(translationType);
            
            for (int i = 0; i < batchInChunk.Count; i++)
            {
                var translation = translations[i];
                float currentStrength = pheromoneGrid[i].pheromoneStrength;
                
                pheromoneGrid[(int) (translation.Value.x + (translation.Value.y * 128))  ] = new Pheromones{pheromoneStrength = math.min(currentStrength + (pheromoneApplicationRate * deltaTime), 100f)};
            }
        }
    }

    protected override void OnUpdate()
    {
        Entity pheromoneEntity = GetSingletonEntity<Pheromones>();
        DynamicBuffer<Pheromones> pheromoneGrid = EntityManager.GetBuffer<Pheromones>(pheromoneEntity);

        EntityQuery query = GetEntityQuery(typeof(Translation), typeof(Ant));
        BufferTypeHandle<Pheromones> pheromoneBufferType = GetBufferTypeHandle<Pheromones>();
        ComponentTypeHandle<Translation> translationType = GetComponentTypeHandle<Translation>(true);

        Entity pheromoneApplicationEntity = GetSingletonEntity<PheromoneApplicationRate>();
        float pheromoneApplicationRate = EntityManager.GetComponentData<PheromoneApplicationRate>(pheromoneApplicationEntity).pheromoneApplicationRate;

        float time = Time.DeltaTime;
        float timeMultiplier = GetSingleton<TimeMultiplier>().SimulationSpeed;
        float scaledTime = time * timeMultiplier;

        BufferJobExample job = new BufferJobExample
        {
            //pheromonesType = pheromoneBufferType,
            pheromoneGrid = pheromoneGrid,
            translationType = translationType,
            pheromoneApplicationRate = pheromoneApplicationRate,
            deltaTime = scaledTime
        };

        Dependency = job.Schedule(query, Dependency);
    }
}
