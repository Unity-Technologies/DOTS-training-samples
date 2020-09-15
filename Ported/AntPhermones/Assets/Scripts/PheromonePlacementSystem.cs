using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class PheromonePlacementSystem : SystemBase {

    protected override void OnUpdate() {
        var mapEntity = GetSingletonEntity<PheromoneMap>();
        var map = EntityManager.GetComponentData<PheromoneMap>(mapEntity);
        var pheromones = EntityManager.GetBuffer<PheromoneStrength>(mapEntity);

        Entities.WithAll<AntTag>().ForEach((in Translation translation) => {
            int2 gridPos = PheromoneMap.WorldToGridPos(map, translation.Value);
            int index = PheromoneMap.GridPosToIndex(map, gridPos);
            pheromones[index] += map.AntPheremoneStrength;
        }).Run();
    }
}