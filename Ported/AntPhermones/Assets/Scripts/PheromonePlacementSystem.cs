using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class PheromonePlacementSystem : SystemBase {

    protected override void OnUpdate() {
        var mapEntity = GetSingletonEntity<PheromoneMap>();
        var map = EntityManager.GetComponentData<PheromoneMap>(mapEntity);
        var pheromones = EntityManager.GetBuffer<PheromoneStrength>(mapEntity);
        var dt = Time.DeltaTime;

        Entities.WithAll<AntTag>().ForEach((in Translation translation) => {
            int2 gridPos = PheromoneMap.WorldToGridPos(map, translation.Value);
            int index = PheromoneMap.GridPosToIndex(map, gridPos);
            if (index < 0 || index > pheromones.Length) {
                UnityEngine.Debug.LogWarning($"Index '{index}' out of bounds '{pheromones.Length}'");
                return;
            }
            pheromones[index] += map.AntPheremoneStrength * dt;
        }).Run();
    }
}