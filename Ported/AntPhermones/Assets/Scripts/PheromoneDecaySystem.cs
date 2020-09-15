using Unity.Entities;
using Unity.Mathematics;

public class PheromoneDecaySystem : SystemBase {

    protected override void OnUpdate() {
        Entities.ForEach((ref DynamicBuffer<PheromoneStrength> pheromones, in PheromoneMap map) => {
            for (int i = 0; i < pheromones.Length; i++) {
                pheromones[i] *= map.PheremoneDecay;
            }
        }).Run();
    }
}