using Unity.Entities;

public class PheromoneVisualizationSystem : SystemBase {
    protected override void OnUpdate() {
        var pv = UnityEngine.Component.FindObjectOfType<PheremoneVisualization>();

        var mapEntity = GetSingletonEntity<PheromoneMap>();
        var map = EntityManager.GetComponentData<PheromoneMap>(mapEntity);
        var pheromones = EntityManager.GetBuffer<PheromoneStrength>(mapEntity);

        pv.SetMap(map, pheromones);
    }
}
