using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

public class PheromoneDecaySystem : SystemBase
{
    protected override void OnCreate()
    {
        RequireSingletonForUpdate<PheromoneStrength>();
    }
    
    protected override void OnUpdate()
    {
        var time = Time.DeltaTime;
        var decayStrength = GetSingleton<Tuning>().PheromoneDecayStrength * time;

        var pheromoneEntity = GetSingletonEntity<PheromoneStrength>();
        DynamicBuffer<PheromoneStrength> pheromoneBuffer = GetBuffer<PheromoneStrength>(pheromoneEntity);

        var bufferAsNativeArray = pheromoneBuffer.AsNativeArray();
        
        var job = new DecayJob(bufferAsNativeArray, decayStrength);
        Dependency = job.Schedule(pheromoneBuffer.Length, 100, Dependency);
    }
    
    [BurstCompile]
    private struct DecayJob : IJobParallelFor
    {
        private NativeArray<PheromoneStrength> pheromoneBuffer;
        private float decayStrength;
        
        public DecayJob(NativeArray<PheromoneStrength> pheromoneBuffer, float decayStrength)
        {
            this.pheromoneBuffer = pheromoneBuffer;
            this.decayStrength = decayStrength;
        }
    
        public void Execute(int index)
        {
            if (pheromoneBuffer[index].Value <= 0) return;
            
            var strength = (float)pheromoneBuffer[index].Value;
            strength -= decayStrength;
            strength = math.max(strength, 0);
            pheromoneBuffer[index] = (byte)strength;
        }
    }
}

