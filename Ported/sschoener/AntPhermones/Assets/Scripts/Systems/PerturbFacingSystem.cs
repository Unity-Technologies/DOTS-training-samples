using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public class PerturbFacingSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        return new PerturbationJob {
            Seed = 1 + (uint)UnityEngine.Time.frameCount,
            Magnitude = 5,
        }.Schedule(this, inputDeps);
    }

    struct PerturbationJob : IJobForEachWithEntity<FacingAngleComponent> {
        public uint Seed;
        public float Magnitude;

        public void Execute(Entity entity, int index, ref FacingAngleComponent facing)
        {
            var rng = new Random((uint)index * Seed);
            facing.Value += rng.NextFloat(-Magnitude, Magnitude);
        }
    }
}
