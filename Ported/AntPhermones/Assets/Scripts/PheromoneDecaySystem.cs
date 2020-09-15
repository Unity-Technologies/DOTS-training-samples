using Unity.Entities;
using Unity.Mathematics;

public class PheromoneDecaySystem : SystemBase {

    protected override void OnUpdate() {
        var dt = Time.DeltaTime;

        Entities.ForEach((ref DynamicBuffer<PheromoneStrength> pheromones, in PheromoneMap map) => {
            var decay = dt * map.PheremoneDecay;

            for (int i = 0; i < pheromones.Length; i++) {
                pheromones[i] -= decay;
            }
        }).Run();
    }
}