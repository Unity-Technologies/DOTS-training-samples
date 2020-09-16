using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class PheromonePlacementSystem : SystemBase {

    protected override void OnUpdate() {
        var mapEntity = GetSingletonEntity<PheromoneMap>();
        var map = EntityManager.GetComponentData<PheromoneMap>(mapEntity);
        var pheromones = EntityManager.GetBuffer<PheromoneStrength>(mapEntity);
        var dt = Time.DeltaTime;

        Entities.ForEach((in AntTag ant, in Translation translation) => {
            int2 gridPos = PheromoneMap.WorldToGridPos(map, translation.Value);
            int index = PheromoneMap.GridPosToIndex(map, gridPos);
            if (index < 0 || index >= pheromones.Length) {
                return;
            }
            pheromones[index] = math.saturate(pheromones[index] + map.AntPheremoneStrength * dt * (ant.HasFood ? 2 : .5f));
        }).Run();
    }
}