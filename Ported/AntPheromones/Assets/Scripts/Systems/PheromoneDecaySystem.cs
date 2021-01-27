using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using System;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
public class PheromoneDecaySystem : SystemBase
{
    float _timeElapsed = 0;

    protected override void OnCreate()
    {
        //var sys = World.GetExistingSystem<FixedStepSimulationSystemGroup>();
        //sys.Timestep = 0.1f;
        RequireSingletonForUpdate<PheromoneStrength>();
        RequireSingletonForUpdate<Tuning>();
    }
    
    protected override void OnUpdate()
    {
        var time = Time.DeltaTime;
        _timeElapsed += Time.DeltaTime;

        var tuning = GetSingleton<Tuning>();

        if (_timeElapsed < tuning.DecayPeriod)
		{
            return;            
		}

        _timeElapsed = 0;

        var decayValue = tuning.DecayValue;

        var pheromoneEntity = GetSingletonEntity<PheromoneStrength>();
        DynamicBuffer<PheromoneStrength> pheromoneBuffer = GetBuffer<PheromoneStrength>(pheromoneEntity);

        var bufferAsNativeArray = pheromoneBuffer.AsNativeArray();
        
        var job = new DecayJob(bufferAsNativeArray, decayValue);
        Dependency = job.Schedule(pheromoneBuffer.Length, 100, Dependency);
    }
    
    [BurstCompile]
    private struct DecayJob : IJobParallelFor
    {
        private NativeArray<PheromoneStrength> pheromoneBuffer;
        private int decayValue;
        
        public DecayJob(NativeArray<PheromoneStrength> pheromoneBuffer, int decayValue)
        {
            this.pheromoneBuffer = pheromoneBuffer;
            this.decayValue = decayValue;
        }
    
        public void Execute(int index)
        {
            if (pheromoneBuffer[index].Value <= 0) return;

            int pVal = pheromoneBuffer[index].Value;
            int newValue = pVal - decayValue;
            newValue = Math.Min(newValue, 255);
            pheromoneBuffer[index] = (byte)newValue;
        }
    }
}

