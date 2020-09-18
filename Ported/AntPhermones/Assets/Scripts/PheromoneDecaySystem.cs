using Unity.Entities;
using Unity.Jobs;
using Unity.Collections;
using Unity.Burst;
using Unity.Transforms;

[UpdateAfter(typeof(TransformSystemGroup))]
public class PheromoneDecaySystem : SystemBase {

    protected override void OnCreate() {
        base.OnCreate();
        RequireForUpdate(GetEntityQuery(typeof(PheromoneMap), typeof(PheromoneStrength)));

    }

    [BurstCompile(CompileSynchronously = true)]
    struct DecayJob : IJobParallelFor
    {
        [ReadOnly] public float decay;
        public NativeArray<PheromoneStrength> pheromones;
        public void Execute(int i)
        {
            pheromones[i] -= decay;
        }
    }

    protected override void OnUpdate() {
        var dt = Time.DeltaTime;

        var mapEntity = GetSingletonEntity<PheromoneMap>();
        var map = EntityManager.GetComponentData<PheromoneMap>(mapEntity);
        var pheromones = EntityManager.GetBuffer<PheromoneStrength>(mapEntity);

        var j = new DecayJob { pheromones = pheromones.AsNativeArray(), decay = map.PheremoneDecay * dt };
        this.Dependency = j.Schedule(pheromones.Length, 64, Dependency);
    }
}
