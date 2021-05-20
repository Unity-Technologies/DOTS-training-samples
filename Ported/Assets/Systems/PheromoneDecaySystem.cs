using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

public class PheromoneDecaySystem : SystemBase
{
    [BurstCompile]
    struct DecayJob : IJobParallelFor
    {
        public NativeArray<float> Data;
        public float dt;
        
        public void Execute(int index)
        {
            Data[index] = Data[index] * (1 - dt / 25);
        }
    }
    
    protected override void OnUpdate()
    {
        var pheromoneMap = GetSingletonEntity<PheromoneMap>();
        var simulationSpeed = GetSingleton<SimulationSpeed>();

        var map = GetBuffer<Pheromone>(pheromoneMap).AsNativeArray().Reinterpret<float>();
        var handle = new DecayJob
        {
            Data = map,
            dt = Time.DeltaTime * simulationSpeed.Value
        }.Schedule(map.Length, 64, Dependency);
        Dependency = JobHandle.CombineDependencies(Dependency, handle);
        Dependency.Complete();
    }
}